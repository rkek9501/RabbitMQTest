using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CommandLine;

using Singleton;

namespace ReceiveTopics {
    class Program {
        private static IModel channel = RabbitMqFactory.GetInstance();
        
        public class Options: Singleton.Options {}

        static void Main(string[] args) {

            channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);
            // declare a server-named queue
            var queueName = channel.QueueDeclare().QueueName;
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                        var routingKey = o.RoutingKey;
                        Console.WriteLine($"RoutingKey is : {routingKey}");
                        channel.QueueBind(queue: queueName,
                                        exchange: "topic_logs",
                                        routingKey: routingKey);
                        var consumer = new EventingBasicConsumer(channel);      
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            // var routingKey = ea.RoutingKey;
                            Console.WriteLine($" [x] Received '{routingKey}' :'{message}'");
                        };
                        channel.BasicConsume(queue: queueName,
                                            autoAck: true,
                                            consumer: consumer);
                        Console.WriteLine("Consumer setting Finished!");
                    });

            Console.ReadLine();
        }
    }
}
