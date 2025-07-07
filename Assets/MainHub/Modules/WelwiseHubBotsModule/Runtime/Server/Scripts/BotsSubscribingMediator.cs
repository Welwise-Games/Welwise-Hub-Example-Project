using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using MainHub.Modules.WelwiseLoadingClothesModule.Runtime.Shared.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotsSubscribingMediator
    {
        private readonly BotsFactory _botsFactory;
        private readonly string _roomSceneName;
        private readonly BotsNicknamesProviderService _botsNicknamesProviderService;
        private readonly BotsCustomizationDataProviderService _botsCustomizationDataProviderService;
        private readonly ServerManager _serverManager;
        private readonly IRoomsProviderService _roomsProviderService;
        private readonly ClientsConfigsProviderService _clientsConfigsProviderService;
        private readonly BotsConfigsProviderService _botsConfigsProviderService;
        private readonly ItemsConfigsProviderService _itemsConfigsProviderService;

        public BotsSubscribingMediator(BotsNicknamesProviderService botsNicknamesProviderService,
            ServerManager serverManager,
            string roomSceneName, BotsFactory botsFactory, ServerSceneManagementService serverSceneManagementService,
            IRoomsProviderService roomsProviderService,
            BotsCustomizationDataProviderService botsCustomizationDataProviderService,
            ClientsConfigsProviderService clientsConfigsProviderService,
            BotsConfigsProviderService botsConfigsProviderService,
            ItemsConfigsProviderService itemsConfigsProviderService)
        {
            _botsNicknamesProviderService = botsNicknamesProviderService;
            _serverManager = serverManager;
            _roomSceneName = roomSceneName;
            _botsFactory = botsFactory;
            _roomsProviderService = roomsProviderService;
            _botsCustomizationDataProviderService = botsCustomizationDataProviderService;
            _clientsConfigsProviderService = clientsConfigsProviderService;
            _botsConfigsProviderService = botsConfigsProviderService;
            _itemsConfigsProviderService = itemsConfigsProviderService;

            _botsFactory.CreatedBotBehaviourController += SubscribeBotBehaviourControllerAsync;
            serverSceneManagementService.ClientLoadedScene += SendInitializationBotsForConnectedClient;
            botsNicknamesProviderService.ChangedBotNickname += SendBotChangedNickname;
            botsCustomizationDataProviderService.ChangedBotCustomizationData += SendBotChangedCustomizationData;
        }

        private void SendBotChangedNickname(int objectId, string nickname)
        {
            var clients = _botsFactory.RoomByBotObjectId[objectId].ConnectedClientsNetworkConnections;
            _serverManager.Broadcast(new HashSet<NetworkConnection>(clients),
                new BotChangedNicknameBroadcast(objectId, nickname));
        }

        private void SendBotChangedCustomizationData(int objectId, CustomizationData customizationData)
        {
            var clients = _botsFactory.RoomByBotObjectId[objectId].ConnectedClientsNetworkConnections;
            _serverManager.Broadcast(new HashSet<NetworkConnection>(clients),
                new BotChangedCustomizationDataBroadcast(objectId, customizationData));
        }

        private void SendInitializationBotsForConnectedClient(NetworkConnection connection, Scene scene)
        {
            if (_roomSceneName != scene.name ||
                !_botsFactory.BotBehaviourControllersByRoom.TryGetValue(
                    _roomsProviderService.RoomsByConnectedClientsNetworkConnections[connection], out var botBehaviourControllers))
                return;

            foreach (var bot in botBehaviourControllers)
            {
                var botObjectId = bot.SerializableComponents.GetComponent<NetworkObject>().ObjectId;
                
                _serverManager.Broadcast(connection,
                    new InitializationBotBroadcast(bot.SerializableComponents.gameObject,
                        _botsNicknamesProviderService.Nicknames[botObjectId],
                        _botsCustomizationDataProviderService.BotsCustomizationData[botObjectId]));
            }
        }

        private async void SubscribeBotBehaviourControllerAsync(BotBehaviourController botBehaviourController,
            IRoom room)
        {
            var botObjectId = botBehaviourController.SerializableComponents.GetComponent<NetworkObject>().ObjectId;

            _botsNicknamesProviderService.AddBotNickname(botObjectId, BotsNicknamesTools.GetRandomNickname());
            
            var clientsConfig = await _clientsConfigsProviderService.GetClientsConfigAsync();
            
            _botsCustomizationDataProviderService.AddBotCustomizationData(botObjectId,
                BotsCustomizationDataTools.GetRandomCustomizationData(
                    new CustomizationData(
                        new ModelAppearanceData(clientsConfig.PlayerDefaultClothesColorValue,
                            clientsConfig.DefaultPlayerSkinColorValue),
                        ClothesLoadingTools
                            .GetDefaultEquippedItemsData()), clientsConfig,
                    (await _botsConfigsProviderService.GetBotsConfigAsync()).SetBotCustomizationDataPartChance,
                    await _itemsConfigsProviderService.GetItemsConfigAsync()));

            botBehaviourController.SerializableComponents.gameObject.GetOrAddComponent<DestroyObserver>().Destroyed +=
                () =>
                {
                    _botsNicknamesProviderService.RemoveBotNickname(botObjectId);
                    _botsCustomizationDataProviderService.RemoveBotCustomizationData(botObjectId);
                };

            botBehaviourController.StartedPlayingEmotion += emotionIndex => _serverManager.Broadcast(
                new HashSet<NetworkConnection>(room.ConnectedClientsNetworkConnections),
                new PlayBotEmotionBroadcast(emotionIndex,
                    botBehaviourController.SerializableComponents.gameObject));
        }
    }
}