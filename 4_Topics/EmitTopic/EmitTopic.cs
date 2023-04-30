using System.Text;
using RabbitMQ.Client;
using CommandLine;

using Singleton;

namespace EmitTopics
{
    class Program
    {
        private static IModel channel = RabbitMqFactory.GetInstance();
        
        public class Options: Singleton.Options
        {
            [Option('m', "message", Required = true, HelpText = "메시지 지정")]
            public String Message { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Console.WriteLine(o.ToString());
                       if (o.Message != null && o.RoutingKey != null) {
                            var message = o.Message;
                            var routingKey = o.RoutingKey;
                            Console.WriteLine($" [x] Sent {message} to {routingKey}");

                            channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

                            var body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "topic_logs",
                                                routingKey: routingKey,
                                                basicProperties: null,
                                                body: body);
                       }
                       
                    })
                    .WithNotParsed(HandleParseError);
            // Console.ReadLine();
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