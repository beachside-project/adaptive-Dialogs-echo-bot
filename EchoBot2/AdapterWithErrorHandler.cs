using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EchoBot2
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration,
            IStorage storage, UserState userState,
            ConversationState conversationState,
            ILogger<BotFrameworkHttpAdapter> logger) : base(configuration, logger)
        {
            Use(new RegisterClassMiddleware<IConfiguration>(configuration));
            this.UseStorage(storage);
            this.UseBotState(userState);
            this.UseBotState(conversationState);

            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user
                await turnContext.SendActivityAsync("The bot encountered an error or bug.");
                await turnContext.SendActivityAsync("To continue to run this bot, please fix the bot source code.");

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }
    }
}
