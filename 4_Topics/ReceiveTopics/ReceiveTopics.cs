using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CommandLine;

using Singleton;

namespace ReceiveTopics {
    class Program {
        static readonly string exchangeName = "header_logs";
        static readonly string exchangeType = ExchangeType.Headers;

        private static IModel channel = RabbitMqFactory.GetInstance();
        
        public class Options: Singleton.Options {}

        static void Main(string[] args) {

            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
            // declare a server-named queue
            var queueName = channel.QueueDeclare().QueueName;
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                        var openedRoutingKey = o.RoutingKey;
                        Console.WriteLine($"OpenedRoutingKey is : {openedRoutingKey}");
                        channel.QueueBind(queue: queueName,
                                        exchange: exchangeName,
                                        routingKey: openedRoutingKey);
                        var consumer = new EventingBasicConsumer(channel);      
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var body2 = ea.Body.ToString;
                            var message = Encoding.UTF8.GetString(body);
                            var routingKey = ea.RoutingKey;
                            Console.WriteLine($" [x] Received body2 '{body2}'");
                            Console.WriteLine($" [x] Received '{openedRoutingKey}(${routingKey})' :'{message}'");
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
