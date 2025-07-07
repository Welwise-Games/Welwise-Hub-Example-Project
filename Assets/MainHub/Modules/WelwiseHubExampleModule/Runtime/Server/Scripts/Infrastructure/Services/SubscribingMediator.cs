using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing.Server;
using UnityEngine.SceneManagement;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network;
using WelwiseHubBotsModule.Runtime.Server.Scripts;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.GameStateMachinePart;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.ChatSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.PlayerSystem;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Holders;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Network;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.HubSystem.Network;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Channel = FishNet.Transporting.Channel;
using SceneManager = FishNet.Managing.Scened.SceneManager;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.Services
{
    public class SubscribingMediator
    {
        private readonly HubsProviderService _hubsProviderService;
        private readonly PlayersFactory _playersFactory;
        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly ClientsCustomizationDataProviderService _clientsCustomizationDataProviderService;
        private readonly EventBus _eventBus;
        private readonly ServerManager _serverManager;

        public SubscribingMediator(ClientsConnectionTrackingServiceForServer clientsConnectionTrackingServiceForServer,
            HubsProviderService hubsProviderService, PlayersFactory playersFactory,
            ServerSceneManagementService serverSceneManagementService,
            ServerChatsDataProvider serverChatsDataProvider,
            ClientsSelectedEmotionsDataProviderService clientsSelectedEmotionsDataProviderService,
            ClientsDataProviderService clientsDataProviderService, EventBus eventBus,
            ClientsCustomizationDataProviderService clientsCustomizationDataProviderService,
            ServerManager serverManager, SceneManager sceneManager,
            BotsFactory botsFactory, BotsConfig botsConfig, IAssetLoader assetLoader)
        {
            _hubsProviderService = hubsProviderService;
            _playersFactory = playersFactory;
            _clientsDataProviderService = clientsDataProviderService;
            _eventBus = eventBus;
            _clientsCustomizationDataProviderService = clientsCustomizationDataProviderService;
            _serverManager = serverManager;

            serverSceneManagementService.ClientLoadedScene += TrySendingInitializationHubDependencies;

            playersFactory.Created += SendInitializationNewClientBroadcast;
            playersFactory.Created += (_, networkConnection)
                =>
            {
                clientsSelectedEmotionsDataProviderService.TryAddingClientSelectedEmotionsData(networkConnection,
                    clientsDataProviderService.Data[networkConnection].SelectedEmotionsData);
            };
            
            // clientsNicknamesProviderService.ChangedClientNickname += (networkConnection, _) =>
            //     clientsDataSavingService.TryAddingDataForSending(networkConnection,
            //         clientsDataProviderService.Data[networkConnection].AccountData);
            //
            // clientsCustomizationDataProviderService.ChangedClientPlayerCustomizationData += (networkConnection, _) =>
            //     clientsDataSavingService.TryAddingDataForSending(networkConnection,
            //         clientsDataProviderService.Data[networkConnection].PlayerCustomizationData);
            //
            clientsCustomizationDataProviderService.ChangedClientPlayerCustomizationData += TrySendingForAllPlayersAboutChangingCustomizationData;
            
            serverManager.RegisterBroadcast<SettingClientCustomizationDataBroadcastForServer>(TrySettingPlayerCustomizationData);
            serverManager.RegisterBroadcast<LoginBroadcast>(EnterInitializationState);
            sceneManager.OnClientPresenceChangeEnd += serverSceneManagementService.TryInvokingClientLoadedScene;

            //clientsSelectedEmotionsDataProviderService.UpdatedData += clientsDataSavingService.TryAddingDataForSending;

            hubsProviderService.HubCreated += hub =>             
                new BotsController(botsFactory, hub, hub.Instance.PortalsTransforms, hub.Instance.ShopTransform,
                    botsConfig, hub.Instance.transform, assetLoader);
            hubsProviderService.HubRemoved += serverChatsDataProvider.TryRemovingHubMessagesData;
            hubsProviderService.HubRemoved += hub => hub.ConnectedClientsNetworkConnections.ForEach(playersFactory.TryRemovingPlayer);

            RegisterClientsConnectionTrackingService(clientsConnectionTrackingServiceForServer, hubsProviderService,
                playersFactory, clientsSelectedEmotionsDataProviderService, clientsDataProviderService);

            serverManager.OnRemoteConnectionState +=
                clientsConnectionTrackingServiceForServer.TryInvokeActionByConnectionState;
        }

        private void TrySettingPlayerCustomizationData(NetworkConnection networkConnection,
            SettingClientCustomizationDataBroadcastForServer broadcast, Channel channel)
            => _clientsCustomizationDataProviderService.TrySettingClientCustomizationData(networkConnection,
                broadcast.CustomizationData);

        private static void RegisterClientsConnectionTrackingService(
            ClientsConnectionTrackingServiceForServer clientsConnectionTrackingServiceForServer,
            HubsProviderService hubsProviderService, PlayersFactory playersFactory,
            ClientsSelectedEmotionsDataProviderService clientsSelectedEmotionsDataProviderService,
            ClientsDataProviderService clientsDataProviderService)
        {
            clientsConnectionTrackingServiceForServer.Disconnected += playersFactory.TryRemovingPlayer;
            clientsConnectionTrackingServiceForServer.Disconnected += hubsProviderService.DisconnectClientFromHub;
            clientsConnectionTrackingServiceForServer.Disconnected +=
                clientsSelectedEmotionsDataProviderService.TryRemovingClientSelectedEmotionsData;
            clientsConnectionTrackingServiceForServer.Disconnected += clientsDataProviderService.TryRemovingClientData;
        }
        
        private void TrySendingForAllPlayersAboutChangingCustomizationData(NetworkConnection changerNetworkConnection, CustomizationData customizationData)
        {
            var hubClientsConnections = _hubsProviderService.HubByPlayerNetworkConnection.GetValueOrDefault(
                changerNetworkConnection)?.ConnectedClientsNetworkConnections;

            if (hubClientsConnections == null)
                return;

            foreach (var clientConnection in hubClientsConnections)
            {
                _serverManager.Broadcast(clientConnection, new SettingClientCustomizationDataBroadcastForClient(
                    customizationData, changerNetworkConnection));
            }
        }

        private void EnterInitializationState(NetworkConnection networkConnection, LoginBroadcast loginBroadcast,
            Channel channel)
        {
            _eventBus.Fire(new EnterServerStateEvent(GameState.Initialization, networkConnection, loginBroadcast.ClientData));
        }

        private async void TrySendingInitializationHubDependencies(NetworkConnection networkConnection,
            Scene scene)
        {
            if (!scene.name.Contains(ScenesNames.Hub)) return;

            await AsyncTools.WaitWhileWithoutSkippingFrame(() => !_playersFactory.CreatedPlayers.ContainsKey(networkConnection));
            
            SendInitializationAllClientsDataBroadcastForNewPlayer(networkConnection);
            SendInitializationAllHubPlayersBroadcastForNewPlayer(networkConnection);
            SendInitializationHubBroadcast(networkConnection);
        }

        private void SendInitializationAllClientsDataBroadcastForNewPlayer(NetworkConnection networkConnection) =>
            _serverManager.Broadcast(networkConnection,
                new ClientsServicesInitializationBroadcast(
                    _hubsProviderService.HubByPlayerNetworkConnection[networkConnection].ConnectedClientsNetworkConnections
                        .Select(connection => new ClientServicesInitializationBroadcast(connection == networkConnection
                            ? _clientsDataProviderService.Data[connection]
                            : GetClientDataForNotOwner(connection), connection)).ToList()));

        private ClientData GetClientDataForNotOwner(NetworkConnection connection) =>
            new()
            {
                AccountData = new ClientAccountData
                    { Nickname = _clientsDataProviderService.Data[connection].AccountData.Nickname },
                CustomizationData = _clientsDataProviderService.Data[connection].CustomizationData
            };

        private void SendInitializationAllHubPlayersBroadcastForNewPlayer(NetworkConnection networkConnection)
        {
            _serverManager.Broadcast(networkConnection, new PlayersInitializationBroadcast(
                _hubsProviderService.HubByPlayerNetworkConnection[networkConnection].ConnectedClientsNetworkConnections
                    .Select(connection =>
                        new PlayerInitializationDependencies(_playersFactory.CreatedPlayers[connection].gameObject,
                            connection)).ToList()));
        }


        private void SendInitializationNewClientBroadcast(SharedPlayerSerializableComponents characterSerializableComponents,
            NetworkConnection newPlayerNetworkConnection)
        {
            _hubsProviderService.HubByPlayerNetworkConnection[newPlayerNetworkConnection].ConnectedClientsNetworkConnections
                .Where(connection => connection != newPlayerNetworkConnection).ForEach(connection =>
                {
                    _serverManager.Broadcast(connection,
                        new ClientServicesInitializationBroadcast(GetClientDataForNotOwner(newPlayerNetworkConnection),
                            newPlayerNetworkConnection));

                    _serverManager.Broadcast(connection,
                        new PlayerInitializationDependencies(characterSerializableComponents.gameObject,
                            newPlayerNetworkConnection));
                });
        }

        private void SendInitializationHubBroadcast(NetworkConnection networkConnection)
        {
            _serverManager.Broadcast(networkConnection,
                new HubInitializationDependencies(_hubsProviderService
                    .HubByPlayerNetworkConnection[networkConnection].Instance.gameObject));
        }
    }
}