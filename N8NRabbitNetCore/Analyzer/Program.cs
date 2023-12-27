// See https://aka.ms/new-console-template for more information
using Analyzer.Models;
using Analyzer.Services.Developer;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Refit;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Analyzer App...");

var connectionFactory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};

using (var connection = connectionFactory.CreateConnection())
using (var analyzeChannel = connection.CreateModel())
using (var detailerChannel = connection.CreateModel())
{
    //analyzeChannel.QueueDeclare(queue: "waiting-for-analyze",
    //                     durable: false,
    //                     exclusive: false,
    //                     autoDelete: false,
    //                     arguments: null);

    //detailerChannel.QueueDeclare(queue: "waiting-for-detail",
    //                     durable: false,
    //                     exclusive: false,
    //                     autoDelete: false,
    //                     arguments: null);

    var consumer = new EventingBasicConsumer(analyzeChannel);


    var timeoutPolicy = Policy
        .Handle<ApiException>(ex => ex.StatusCode == HttpStatusCode.RequestTimeout)
        .RetryAsync(3, async (exception, retryCount) =>
            await Task.Delay(1000).ConfigureAwait(false));


    consumer.Received += async (model, mq) =>
    {
        var body = mq.Body.ToArray();
        var username = Encoding.UTF8.GetString(body);
        Console.WriteLine($"Received: {username}");

        var api = RestService.For<IDeveloperAPI>(new HttpClient(new AuthenticatedHttpClientHandler(() =>
        {
            var tokenResult = RestService.For<IDeveloperAPI>("http://localhost:5020").GetToken(new Analyzer.Services.Developer.DTOs.GetTokenCommand()
            {
                Username = "admin",
                Password = "admin"
            }).GetAwaiter().GetResult();

            return Task.FromResult(tokenResult.AccessToken);
        }))
        {
            BaseAddress = new Uri("http://localhost:5020")
        });

        Analyzer.Services.Developer.DTOs.DeveloperDetailResponse developerResult = null;

        try
        {
            developerResult = await timeoutPolicy
            .ExecuteAsync(async () => await api.DeveloperDetail(new Analyzer.Services.Developer.DTOs.DeveloperDetailQuery()
            {
                Username = username
            }))
            .ConfigureAwait(false);

            var developer = new Developer()
            {
                Fullname = developerResult.Fullname,
                Username = developerResult.Username,
                Projects = developerResult is null ? new List<Project>() : developerResult.Projects.Select(z => new Project()
                {
                    Languages = z.Languages,
                    Name = z.Name
                }).ToList()
            };

            string developerJson = JsonSerializer.Serialize(developer);
            detailerChannel.BasicPublish(exchange: "",
                                 routingKey: "waiting-for-detail",
                                 basicProperties: null,
                                 body: Encoding.UTF8.GetBytes(developerJson));
        }
        catch(ApiException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Content);
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    };

    analyzeChannel.BasicConsume(queue: "waiting-for-analyze",
                         autoAck: true,
                         consumer: consumer);
    Console.ReadLine();
}

class AuthenticatedHttpClientHandler : HttpClientHandler
{
    private readonly Func<Task<string>> getToken;
    public AuthenticatedHttpClientHandler(Func<Task<string>> getToken)
    {
        if (getToken == null) throw new ArgumentNullException(nameof(getToken));
        this.getToken = getToken;
    }

    async protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var auth = request.Headers.Authorization;
        if (auth != null)
        {
            var token = await getToken().ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}

class PollyHandler : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;

    public PollyHandler(IAsyncPolicy<HttpResponseMessage> policy)
    {
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        return _policy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
    }
}