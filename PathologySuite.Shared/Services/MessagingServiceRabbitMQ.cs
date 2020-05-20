using PathologySuite.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Services
{
    public class MessagingServiceRabbitMQ : IMessagingService
    {
        private string _hostname;
        public MessagingServiceRabbitMQ(string hostname)
        {
            _hostname = hostname;
        }

        public void ExchangeDeclare(string name)
        {
            
        }
    }
}
