using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        public Action<string, string> OnCallBack { get; set; }
        public string StatusText { get; set; }
        public void SignIn(string userName)
        {
            UserName = userName;
            //Connect to server (use async method to avoid blocking UI thread)
            if (!String.IsNullOrEmpty(UserName))
            {
                //StatusText.Visibility = Visibility.Visible;
                //StatusText.Content = "Connecting to server...";
                ConnectAsync();
            }
        }
        public void Send(string message)
        {
            HubProxy.Invoke("Send", UserName, message); 
        }
        public async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);
            Connection.Closed += ConnectionClosed;
            HubProxy = Connection.CreateHubProxy("MessageHub");
            //Handle incoming event from server: use Invoke to write to console from SignalR's thread
            HubProxy.On<string, string>("Send", (name, message) => OnCallBack(name, message));
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

            //Show chat UI; hide login UI
            //SignInPanel.Visibility = Visibility.Collapsed;
            //ChatPanel.Visibility = Visibility.Visible;
            //ButtonSend.IsEnabled = true;
            //TextBoxMessage.Focus();
            //RichTextBoxConsole.AppendText("Connected to server at " + ServerURI + "\r");
        }

    }
}
