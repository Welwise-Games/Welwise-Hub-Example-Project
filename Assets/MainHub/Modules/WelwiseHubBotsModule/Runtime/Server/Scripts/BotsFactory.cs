using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Object;
using UnityEngine;
using UnityEngine.SceneManagement;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseCharacterModule.Runtime.Shared.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;
using Object = UnityEngine.Object;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotsFactory
    {
        public event Action<BotBehaviourController, IRoom> CreatedBotBehaviourController;

        public IReadOnlyDictionary<IRoom, HashSet<BotBehaviourController>> BotBehaviourControllersByRoom =>
            _botBehaviourControllersByRoom;

        public IReadOnlyDictionary<int, IRoom> RoomByBotObjectId => _roomByBotObjectId;

        private readonly BotsConfigsProviderService _botsConfigsProviderService;
        private readonly SetPlayerAnimationPlaceModelsProviderService _setPlayerAnimationPlaceModelsProviderService;
        private readonly EmotionsConfigsProviderService _emotionsConfigsProviderService;
        private readonly IRoomsProviderService _roomsProviderService;
        private readonly ServerSetPlayersAnimationsPlacesSynchronizer _serverSetPlayersAnimationsPlacesSynchronizer;
        private readonly BotsNicknamesProviderService _botsNicknamesProviderService;
        private readonly BotsCustomizationDataProviderService _botsCustomizationDataProviderService;

        private readonly Dictionary<IRoom, HashSet<BotBehaviourController>> _botBehaviourControllersByRoom =
            new Dictionary<IRoom, HashSet<BotBehaviourController>>();

        private readonly Dictionary<int, IRoom> _roomByBotObjectId = new Dictionary<int, IRoom>();

        private readonly ClientsConfigsProviderService _clientsConfigsProviderService;
        private readonly ItemsConfigsProviderService _itemsConfigsProviderService;

        private readonly Container _container = new Container();

        public const string BotAssetId =
#if ADDRESSABLES
            "Bot";
#else
            "WelwiseHubBotsModule/Runtime/Shared/Loadable/Bot";
#endif

        public BotsFactory(BotsConfigsProviderService botsConfigsProviderService,
            SetPlayerAnimationPlaceModelsProviderService setPlayerAnimationPlaceModelsProviderService,
            IRoomsProviderService roomsProviderService,
            EmotionsConfigsProviderService emotionsConfigsProviderService,
            ServerSetPlayersAnimationsPlacesSynchronizer serverSetPlayersAnimationsPlacesSynchronizer,
            BotsNicknamesProviderService botsNicknamesProviderService,
            BotsCustomizationDataProviderService botsCustomizationDataProviderService,
            ClientsConfigsProviderService clientsConfigsProviderService,
            ItemsConfigsProviderService itemsConfigsProviderService)
        {
            _botsConfigsProviderService = botsConfigsProviderService;
            _setPlayerAnimationPlaceModelsProviderService = setPlayerAnimationPlaceModelsProviderService;
            _roomsProviderService = roomsProviderService;
            _emotionsConfigsProviderService = emotionsConfigsProviderService;
            _serverSetPlayersAnimationsPlacesSynchronizer = serverSetPlayersAnimationsPlacesSynchronizer;
            _botsNicknamesProviderService = botsNicknamesProviderService;
            _botsCustomizationDataProviderService = botsCustomizationDataProviderService;
            _clientsConfigsProviderService = clientsConfigsProviderService;
            _itemsConfigsProviderService = itemsConfigsProviderService;
        }

        public async UniTask<BotBehaviourController> GetInitializedBotControllerAsync(IRoom room,
            Transform[] portalsTransforms, Transform shopTransform, Scene scene, IAssetLoader assetLoader)
        {
            var prefab =
                await _container.GetOrLoadAndRegisterObjectAsync<SharedBotSerializableComponents>(BotAssetId,
                    assetLoader,
                    shouldCreate: false);

            var botsConfig = await _botsConfigsProviderService.GetBotsConfigAsync();

            var serializableComponents = Object.Instantiate(prefab, botsConfig.SpawnPosition, Quaternion.identity);

            InstanceFinder.ServerManager.Spawn(serializableComponents.gameObject, null, scene);

            var botModel = new BotBehaviourModel(botsConfig,
                new Timer(serializableComponents.destroyCancellationToken),
                new Timer(serializableComponents.destroyCancellationToken));

            var botBehaviourController = new BotBehaviourController(serializableComponents, botModel,
                _setPlayerAnimationPlaceModelsProviderService,
                room, portalsTransforms, shopTransform, _emotionsConfigsProviderService,
                _serverSetPlayersAnimationsPlacesSynchronizer, _botsNicknamesProviderService,
                _botsCustomizationDataProviderService,
                _clientsConfigsProviderService, _itemsConfigsProviderService);

            var heroAnimatorController =
                new HeroAnimatorController(serializableComponents.HeroAnimatorSerializableComponents);

            new BotAnimatorController(heroAnimatorController, botBehaviourController);

            botBehaviourController.EnteredPortal +=
                () => InstanceFinder.ServerManager.Despawn(serializableComponents.gameObject);

            var botObjectId = serializableComponents.GetComponent<NetworkObject>().ObjectId;

            CreatedBotBehaviourController?.Invoke(botBehaviourController, room);

            foreach (var connection in room.ConnectedClientsNetworkConnections)
            {
                if (connection.Scenes.Contains(scene))
                    InstanceFinder.ServerManager.Broadcast(
                        connection, new InitializationBotBroadcast(serializableComponents.gameObject,
                            _botsNicknamesProviderService.Nicknames[botObjectId],
                            _botsCustomizationDataProviderService.BotsCustomizationData[botObjectId]));
            }

            if (!_botBehaviourControllersByRoom.ContainsKey(room))
            {
                _botBehaviourControllersByRoom.Add(room, new HashSet<BotBehaviourController>());

                _roomsProviderService.RoomRemoved += removedRoom =>
                    _botBehaviourControllersByRoom.Remove(removedRoom);
            }

            _roomByBotObjectId.Add(botObjectId, room);

            botBehaviourController.SerializableComponents.gameObject.GetOrAddComponent<DestroyObserver>().Destroyed +=
                () =>
                {
                    botBehaviourController.Dispose();
                    _botBehaviourControllersByRoom.GetValueOrDefault(room)?.Remove(botBehaviourController);
                    _roomByBotObjectId.Remove(botBehaviourController.SerializableComponents
                        .GetComponent<NetworkObject>().ObjectId);
                };

            _botBehaviourControllersByRoom[room].Add(botBehaviourController);


            return botBehaviourController;
        }
    }
}