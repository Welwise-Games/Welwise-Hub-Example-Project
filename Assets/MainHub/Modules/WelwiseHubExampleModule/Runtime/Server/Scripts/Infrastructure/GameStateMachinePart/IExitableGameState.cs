using Cysharp.Threading.Tasks;
using FishNet.Connection;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.GameStateMachinePart
{
    public interface IExitableGameState
    {
        UniTask ExitAsync(NetworkConnection networkConnection);
    }
}