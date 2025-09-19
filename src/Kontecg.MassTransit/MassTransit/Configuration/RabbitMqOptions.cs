using System.Collections.Generic;

namespace Kontecg.MassTransit.Configuration
{
    public class RabbitMqOptions
    {
        public string Host { get; set; } = "rabbitmq://localhost";

        public int Port { get; set; } = 5672;

        public string VirtualHost { get; set; } = "/";

        public string Username { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public int HeartbeatInterval { get; set; } = 60;

        public string QueueName { get; set; } = "kontecg";

        public List<string> ClusterNodes { get; } = new();
    }
}
