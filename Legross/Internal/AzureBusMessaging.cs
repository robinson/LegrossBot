using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Legross.Internal
{
    public class AzureBusMessaging
    {
        static QueueClient _client;
        public AzureBusMessaging() {
            
            var connectionString = "Endpoint=sb://legrossbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Z8pJ72dqjkQ2JvAokyoeNctcRt28jXb2fj1wNwOalVw=";
            var queueName = "LegrossBusQueue1";
            _client = QueueClient.CreateFromConnectionString(connectionString, queueName);
        }
        public void Send(string message)
        {
            //var connectionString = "Endpoint=sb://legrossbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Z8pJ72dqjkQ2JvAokyoeNctcRt28jXb2fj1wNwOalVw=";
            //var queueName = "LegrossBusQueue1";

            //var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var sendMessage = new BrokeredMessage(message);
            _client.Send(sendMessage);
        }
        
    }
}