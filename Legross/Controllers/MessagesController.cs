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
using System.Collections.Generic;

namespace Legross
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static HubClient hubClient;
        static string ClientId = ConfigurationManager.AppSettings["ClientId"];
        static string serviceUrl;
        static string conversationId;
        static string botId;
        static string botName;
        static string recipientId;
        static string recipientName;
        static string channelId;

        public MessagesController()
        {
            if (hubClient == null)
            {
                hubClient = new HubClient();
                hubClient.SignIn("testAccount");
                hubClient.OnCallBack = (string who, string message, string conId) => OnHubCallBack(who, message, conId);
                //serviceUrl = string.Empty;
            }
        }
        void OnHubCallBack(string who, string message, string conId)
        {
            var connector = new ConnectorClient(new Uri(serviceUrl));

            var conversation = new ConversationAccount(true, conversationId);
            var botAccount = new ChannelAccount(botId, botName);

            IMessageActivity replyMessage = Activity.CreateMessageActivity();
            replyMessage.From = botAccount;
            replyMessage.Conversation = conversation;
            replyMessage.ChannelId = channelId;
            replyMessage.Recipient = new ChannelAccount(recipientId, recipientName);
            replyMessage.Text = message;
            connector.Conversations.SendToConversation((Activity)replyMessage);
            //var myConnector = new ConnectorClient(new Uri(serviceUrl));
            //IMessageActivity newMessage = Activity.CreateMessageActivity();
            //newMessage.Type = ActivityTypes.Message;
            //newMessage.From = botAccount;
            //newMessage.Conversation = conversation;
            //newMessage.Recipient = new ChannelAccount("reply from bo");
            //newMessage.Text = "Yo yo yo!";
            //await connector.Conversations.SendToConversationAsync((Activity)newMessage);


            //var clientConnector = new ConnectorClient(new Uri(serviceUrl));
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
                //serviceUrl = activity.ServiceUrl;
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                try
                {
                    //send broadcast to client

                    //var context = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
                    //context.Clients.All.SendChatMessage("ChatClient", activity.Text);
                    hubClient.Send("ChatClient", activity.Text, conversationId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                serviceUrl = activity.ServiceUrl;
                conversationId = activity.Conversation.Id;
                botId = activity.Recipient.Id;
                botName = activity.Recipient.Name;
                recipientId = activity.From.Id;
                recipientName = activity.From.Name;
                channelId = activity.ChannelId;

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