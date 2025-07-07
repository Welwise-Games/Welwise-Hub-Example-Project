using Cysharp.Threading.Tasks;
using FishNet.Connection;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.GameStateMachinePart
{
    public interface IGameState<TPayload> : IExitableGameState
    {
        UniTask EnterAsync(NetworkConnection networkConnection, TPayload payload);
    }
    
    public interface IGameState : IExitableGameState
    {
        UniTask EnterAsync(NetworkConnection networkConnection);
    }
}