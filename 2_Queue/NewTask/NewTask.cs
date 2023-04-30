using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory {
    HostName = "localhost",
    Port = 5672,
    UserName = "username",
    Password = "password"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "task_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var message = GetMessage(args);

var properties = channel.CreateBasicProperties();
properties.Persistent = true;

for (var i = 0; i< 10; i++) {
    var body = Encoding.UTF8.GetBytes(message + (i+1));
    channel.BasicPublish(exchange: string.Empty,
                        routingKey: "task_queue",
                        basicProperties: properties,
                        body: body);
    Console.WriteLine($" [x] Sent {message}");
}


// Console.WriteLine(" Press [enter] to exit.");
// Console.ReadLine();

static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
}