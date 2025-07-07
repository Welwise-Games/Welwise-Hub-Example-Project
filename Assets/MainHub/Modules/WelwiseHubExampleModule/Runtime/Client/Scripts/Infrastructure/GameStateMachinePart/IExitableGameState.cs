using Cysharp.Threading.Tasks;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public interface IExitableGameState
    {
        UniTask ExitAsync();
    }
}