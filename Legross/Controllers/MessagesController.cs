using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Legross.Dialogs;
using Microsoft.ServiceBus.Messaging;
using Legross.Hubs;
using Microsoft.AspNet.SignalR;
using System.Configuration;

namespace Legross
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private ConnectorClient connector;
        private static HubClient hubClient;
        private static string serviceUrl;
        private Activity act = new Activity();
        static string ClientId = ConfigurationManager.AppSettings["ClientId"];
        public MessagesController()
        {
            if (hubClient == null)
            {
                hubClient = new HubClient();
                hubClient.SignIn("testAccount");
                hubClient.OnCallBack = (string who, string message) => OnHubCallBack(who, message);
                serviceUrl = string.Empty;
            }
        }
        void OnHubCallBack(string who, string message)
        {
            var clientConnector = new ConnectorClient(new Uri(serviceUrl));
            var replyMessage = act.CreateReply(message);
            connector.Conversations.ReplyToActivityAsync(replyMessage);
            //int length = (activity.Text ?? string.Empty).Length;
            //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
            //await connector.Conversations.ReplyToActivityAsync(reply);
        }
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                act = activity;
                serviceUrl = activity.ServiceUrl;
                connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return

                try
                {
                    //send broadcast to client

                    //var context = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
                    //context.Clients.All.SendChatMessage("ChatClient", activity.Text);
                    hubClient.Send(activity.Text, "ChatClient");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                // return our reply to the user
                await ResponseReplyMessage(connector, activity);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        private async Task ResponseReplyMessage(ConnectorClient connector, Activity activity)
        {
            int length = (activity.Text ?? string.Empty).Length;
            Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}