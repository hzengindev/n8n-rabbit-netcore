using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Detailer App...");

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest",
};

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "waiting-for-detail",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, mq) =>
    {
        var body = mq.Body.ToArray();
        var developerJson = Encoding.UTF8.GetString(body);
        Console.WriteLine($"Received: {developerJson}");

        var logMessage = JsonSerializer.Deserialize<Developer>(developerJson);

        // TODO: add more detail operation
    };

    channel.BasicConsume(queue: "waiting-for-detail",
                         autoAck: true,
                         consumer: consumer);
    Console.ReadLine();
}

public class Developer
{
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("fullname")]
    public string Fullname { get; set; }
    [JsonPropertyName("projects")]
    public List<Project> Projects { get; set; }
}

public class Project
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("languages")]
    public string Languages { get; set; }
}