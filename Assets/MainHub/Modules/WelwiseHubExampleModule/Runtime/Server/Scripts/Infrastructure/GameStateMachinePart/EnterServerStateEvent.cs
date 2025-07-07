using FishNet.Connection;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.GameStateMachinePart
{
    public struct EnterServerStateEvent : IEvent
    {
        public readonly GameState GameState;
        public readonly NetworkConnection Connection;
        public readonly object Dependency;

        public EnterServerStateEvent(GameState gameState, NetworkConnection connection, object dependency = null)
        {
            GameState = gameState;
            Connection = connection;
            Dependency = dependency;
        }
    }
}