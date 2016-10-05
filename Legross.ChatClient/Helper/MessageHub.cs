using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Legross.ChatClient.Helper
{
    public class MessageHub
    {
        /// <summary>
        /// This name is simply added to sent messages to identify the user; this 
        /// sample does not include authentication.
        /// </summary>
        public String UserName { get; set; }
        public IHubProxy HubProxy { get; set; }
        static string ServerURI = ConfigurationManager.AppSettings["ServerURI"];
        public HubConnection Connection { get; set; }
        public Action ConnectionClosed { get; set; }
        public Action<string, string, string> OnCallBack { get; set; }
        public string StatusText { get; set; }
        public void SignIn(string userName)
        {
            UserName = userName;
            if (!String.IsNullOrEmpty(UserName))
            {  
                ConnectAsync();
            }
        }
        public void Send(string to, string message,string conversationId)
        {
            HubProxy.Invoke("SendChatMessage", new object[] { to, message, "ChatClient", conversationId });
        }
        public async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI, "UserName=ChatClient");
            Connection.Closed += ConnectionClosed;
            HubProxy = Connection.CreateHubProxy("MessageHub");
            //Handle incoming event from server: use Invoke to write to console from SignalR's thread
            HubProxy.On<string, string, string>("addChatMessage", (who, message, conversationId) => OnCallBack(who, message, conversationId));
            try
            {
                await Connection.Start();
                
            }
            catch (HttpRequestException)
            {
                StatusText = "Unable to connect to server: Start server before connecting clients.";
                //No connection: Don't enable Send button or show chat UI
                return;
            }
        }

    }
}
