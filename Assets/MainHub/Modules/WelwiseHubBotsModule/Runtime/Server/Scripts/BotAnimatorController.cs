using WelwiseCharacterModule.Runtime.Shared.Scripts;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotAnimatorController
    {
        public BotAnimatorController(HeroAnimatorController heroAnimatorController,
            BotBehaviourController botBehaviourController)
        {
            botBehaviourController.ChangedRunningState += heroAnimatorController.SetIsRunning;
            heroAnimatorController.SetIsRunning(!botBehaviourController.IsInteracting);
        }
    }
}