using System.Collections.Generic;
using System.Linq;
using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network.Dependencies;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network
{
    public class ServerEmotionsPlayingSynchronizerService
    {
        private readonly ClientsSelectedEmotionsDataProviderService _clientsSelectedEmotionsDataProviderService;
        private readonly IVisibleClientsProviderService _visibleClientsProviderService;
        private readonly ServerManager _serverManager;
        private readonly EmotionsConfigProviderService _emotionsConfigProviderService;
        
        public ServerEmotionsPlayingSynchronizerService(
            ClientsSelectedEmotionsDataProviderService clientsSelectedEmotionsDataProviderService,
            IVisibleClientsProviderService visibleClientsProviderService, ServerManager serverManager, EmotionsConfigProviderService emotionsConfigProviderService)
        {
            _clientsSelectedEmotionsDataProviderService = clientsSelectedEmotionsDataProviderService;
            _visibleClientsProviderService = visibleClientsProviderService;
            _serverManager = serverManager;
            _emotionsConfigProviderService = emotionsConfigProviderService;

            serverManager.RegisterBroadcast<PlayingEmotionAnimationDependenciesForServer>(
                HandlePlayingAnimationAsync);

            _clientsSelectedEmotionsDataProviderService.AddedData += SendUpdateSelectedEmotionsDataBroadcast;
            _clientsSelectedEmotionsDataProviderService.UpdatedData += SendUpdateSelectedEmotionsDataBroadcast;
        }

        private void SendUpdateSelectedEmotionsDataBroadcast(NetworkConnection networkConnection,
            SelectedEmotionsData data) => _serverManager.Broadcast(networkConnection, new UpdateEmotionsDataBroadcastForClient(data));

        public async void HandlePlayingAnimationAsync(NetworkConnection serverNetworkConnection,
            PlayingEmotionAnimationDependenciesForServer selectedEmotionsDependenciesForServer, Channel channel)
        {
            var emotionIndex = _clientsSelectedEmotionsDataProviderService.ClientsData
                .GetValueOrDefault(serverNetworkConnection)?.SelectedItemsData
                .SafeGet(selectedEmotionsDependenciesForServer.EmotionOrdinalIndex)?.Index;

            if (emotionIndex == null)
                return;
            
            var config =
                (await _emotionsConfigProviderService.GetEmotionsAnimationsConfig()).Configs
                    .FirstOrDefault(config => config.Index == emotionIndex);
            
            if (config == null)
                return;

            var visibleClients = _visibleClientsProviderService.GetVisibleClientsForClient(serverNetworkConnection);

            visibleClients?.ForEach(clientConnection => _serverManager.Broadcast(clientConnection, new
                PlayingEmotionAnimationDependenciesForClient(emotionIndex, serverNetworkConnection)));
        }
    }
}