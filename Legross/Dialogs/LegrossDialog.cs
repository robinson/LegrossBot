using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis.Models;

using System.Threading;
using Microsoft.Bot.Connector;

namespace Legross.Dialogs
{
    [LuisModel("a0101534-3872-4263-b65d-36e29279d6f6", "fe24da2bdad14b25842661e0eb4c2add")]
    [Serializable]
    public class LegrossDialog : LuisDialog<object>
    {
        public LegrossDialog(params ILuisService[] services) : base(services)
        {
        }
        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            var cts = new CancellationTokenSource();
            await context.Forward(new GreetingsDialog(), GreetingDialogDone, await message, cts.Token);
        }

        private async Task GreetingDialogDone(IDialogContext context, IAwaitable<bool> result)
        {
            var success = await result;
            if (!success)
                await context.PostAsync("Xin loi, ban noi clgt?.");

            context.Wait(MessageReceived);
        }

    }
}