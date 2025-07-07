using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing.Server;
using Modules.WelwiseChatModule.Runtime.Server.Scripts;
using Modules.WelwiseChatModule.Runtime.Server.Scripts.Network;
using UnityEngine.SceneManagement;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.ChatSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.PlayerSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.GameStateMachinePart
{
    public class HubGameState : IGameState
    {
        private readonly PlayersFactory _playersFactory;
        private readonly HubsProviderService _hubsProviderService;
        private readonly IServerChatsDataProvider _serverChatsDataProvider;
        private readonly ServerManager _serverManager;

        public HubGameState(PlayersFactory playersFactory, HubsProviderService hubsProviderService,
            ServerChatsDataProvider serverChatsDataProvider, ServerManager serverManager)
        {
            _playersFactory = playersFactory;
            _hubsProviderService = hubsProviderService;
            _serverChatsDataProvider = serverChatsDataProvider;
            _serverManager = serverManager;
        }

        public async UniTask EnterAsync(NetworkConnection networkConnection)
        {
            var playerSceneContainer = new DataContainer<Scene>();
            await _hubsProviderService.TryGettingAndAddingPlayerToFreeHubAsync(networkConnection, playerSceneContainer);
            _playersFactory.GetInitializedOwnerPlayerAsync(networkConnection, playerSceneContainer.Data).Forget();

            _serverChatsDataProvider.SendInitializationChatsDataForClient(networkConnection, _serverManager);
        }

        public async UniTask ExitAsync(NetworkConnection networkConnection)
        {
        }
    }
}