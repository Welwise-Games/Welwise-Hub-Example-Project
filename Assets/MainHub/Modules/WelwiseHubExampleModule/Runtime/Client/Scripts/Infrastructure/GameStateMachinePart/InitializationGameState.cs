using Cysharp.Threading.Tasks;
using FishNet.Managing.Client;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public class InitializationGameState : IGameState
    {
        private readonly ClientManager _clientManager;

        public InitializationGameState(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public async UniTask EnterAsync() => await NetworkTools.StartClientConnection(_clientManager);

        public async UniTask ExitAsync() { }
    }
}