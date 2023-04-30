using System;
using RabbitMQ.Client;
using CommandLine;

namespace Singleton
{
    public class RabbitMqFactory
    {
        private RabbitMqFactory() { }

        private static IModel _instance;
        private static ConnectionFactory _factory;
        private static IConnection _connection;

        private static readonly object _lock = new object();

        public static IModel GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _factory = new ConnectionFactory {
                            HostName = "localhost",
                            Port = 5672,
                            UserName = "username",
                            Password = "password"
                        };
                        _connection = _factory.CreateConnection();
                        _instance = _connection.CreateModel();
                    }
                }
            }
            return _instance;
        }    
    }
    public class Options
    {
        [Option('k', "routingkey", Required = false, Default = "#", HelpText = "라우팅 키")]
        public String RoutingKey { get; set; }
    }
}