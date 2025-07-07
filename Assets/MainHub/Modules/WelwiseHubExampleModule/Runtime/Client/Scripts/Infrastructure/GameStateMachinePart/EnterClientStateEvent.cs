using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public struct EnterClientStateEvent : IEvent
    {
        public readonly GameState GameState;

        public EnterClientStateEvent(GameState gameState)
        {
            GameState = gameState;
        }
    }
}