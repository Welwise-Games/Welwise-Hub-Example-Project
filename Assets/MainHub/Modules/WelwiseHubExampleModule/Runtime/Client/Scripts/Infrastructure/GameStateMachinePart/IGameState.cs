using Cysharp.Threading.Tasks;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public interface IGameState : IExitableGameState
    {
        UniTask EnterAsync();
    }
}