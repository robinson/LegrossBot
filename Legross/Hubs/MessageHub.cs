using Legross.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Legross.Hubs
{
    [HubName("MessageHub")]
    public class MessageHub : Hub
    {
        //public void Send(string userId, string message)
        //{
        //    // Call the broadcastMessage method to update clients.
        //    //Clients.All.broadcastMessage(name, message);
        //    // Call the addNewMessageToPage method to update clients.
        //    //Clients.All.addNewMessageToPage(name, message);
        //    Clients.User(userId).send(message);
        //}
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public void SendChatMessage(string to, string message, string from, string conversationId)
        {
            foreach (var connectionId in _connections.GetConnections(to))
            {
                Clients.Client(connectionId).addChatMessage(from, message, conversationId);
            }
        }

        public override Task OnConnected()
        {
            //string name = Context.User.Identity.Name;
            var name = Context.QueryString["UserName"];

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var name = Context.QueryString["UserName"];

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var name = Context.QueryString["UserName"];

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }


    }
}