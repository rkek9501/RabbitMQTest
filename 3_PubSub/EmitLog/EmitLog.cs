using System.Text;
using RabbitMQ.Client;
using CommandLine;

namespace QuickStart
{
    class Program
    {
        
        public class Options
        {
            [Option('m', "message", Required = true, HelpText = "메시지 지정")]
            public String Message { get; set; }

            [Option('k', "routingkey", Required = true, HelpText = "라우팅 키")]
            public String RoutingKey { get; set; }
        }

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory {
                HostName = "localhost",
                Port = 5672,
                UserName = "username",
                Password = "password"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            Console.WriteLine("01234");

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Console.WriteLine(o.ToString());
                       if (o.Message != null && o.RoutingKey != null) {
                            var message = o.Message;
                            var routingKey = o.RoutingKey;
                            Console.WriteLine($" [x] Sent {message} to {routingKey}");

                            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                            var body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "logs",
                                                routingKey: routingKey,
                                                basicProperties: null,
                                                body: body);
                       }
                       
                    })
                    .WithNotParsed(HandleParseError);
            Console.ReadLine();
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("1234");
            Console.WriteLine(errs.ToString());
        }
        static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
        }
    }
}