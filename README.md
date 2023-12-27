# n8n-rabbit-netcore

1. MainAPI
    1. N8N üzerindeki webhook’a http call yapar
    2. Refit kullanır
    3. 1 tane endpoint içerir
        1. api/developer/{username}
2. N8N süreci başlar
    1. N8N üzerinde bir webhook username parametresi bekler
    2. Basic bir auto tanımı içerir, http request üzerinde Authorization heder ile 123456 parametresi alır
    3. JS script injection, Okuduğu değer içinde username bilgisini js fonksiyondan geçirerek  lowercase uygun hale getirir (örnek bir ara süreç)
    4. Queue management, JS adımından geçen değer rabbitmq plugin ile waiting-for-analyze kuyruğuna yazılır
3. Consumer/Producer console app süreci başlar (Analyzer/Detailer)
    1. Analyzer Console uygulaması waiting-for-anaylze kuyruğunu dinler
        1. Gelen username bilgisini çeker
        2. Refit ve Polly kullanarak DeveloperAPI’ye http istek atar.
            1. Refit ile API yönetimi ve Authentication token yönetimi yapılır
            2. Polly ile retry policy tanımı ve yönetimi yapılır.
        3. API üzerinden gelen developer detay json result, waiting-for-detailer kuyruğuna yazılır
    2. Detailer Console uygulaması waiting-for-detailer kuyruğunu dinler
        1. Bir önceki producer console uygulamasında gelen değer işler.
        2. Burada yapılan ek bir işlem yok, sadece kuyruklar arası geçişlerden sonra devam eden async bir process’i temsil eder.
4. DeveloperAPI
    1. JWT Auth alt yapısı içerir
    2. Temel endpoint gereksinimlerini karşılamak için inmemory data içerir
    3. Global Exception Handling middleware ile process takibi yapılır ve loglama yapılır
    4. Request Logging Middleware ile gelen istekler SeriLog ile SEQ üzerinde loglanır.
    5. Custom authorize attribute ile endpoint erişim durumu kontrol edilir.
    6. MediatR ile temel CQRS uygulaması içerir.

![cover](https://github.com/hzengindev/n8n-rabbit-netcore/blob/main/assets/n8n.png?raw=true)
