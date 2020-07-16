using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot2.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly ResourceExplorer _resourceExplorer;
        private DialogManager _dialogManager;

        public EchoBot(ResourceExplorer resourceExplorer)
        {
            _resourceExplorer = resourceExplorer;
            LoadRootDialog();

            _resourceExplorer.Changed += (e, resources) =>
            {
                if (resources.Any(resource => resource.Id.EndsWith(".dialog")))
                {
                    Task.Run(LoadRootDialog);
                }
            };
        }

        private void LoadRootDialog()
        {
            var resource = _resourceExplorer.GetResource("root.dialog");
            _dialogManager = new DialogManager(_resourceExplorer.LoadType<AdaptiveDialog>(resource));
            _dialogManager.UseResourceExplorer(_resourceExplorer);
            _dialogManager.UseLanguageGeneration();
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await _dialogManager.OnTurnAsync(turnContext, cancellationToken).ConfigureAwait(false);
        }
    }
}