using WelwiseCharacterModule.Runtime.Shared.Scripts;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotAnimatorController
    {
        public BotAnimatorController(HeroAnimatorController heroAnimatorController,
            BotController botController)
        {
            botController.ChangedRunningState += heroAnimatorController.SetIsRunning;
            heroAnimatorController.SetIsRunning(!botController.IsInteracting);
        }
    }
}