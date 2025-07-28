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
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;
using Object = UnityEngine.Object;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotsFactory
    {
        public event Action<BotController, IRoom> CreatedBotBehaviourController;

        public IReadOnlyDictionary<IRoom, HashSet<BotController>> BotBehaviourControllersByRoom =>
            _botBehaviourControllersByRoom;

        public IReadOnlyDictionary<int, IRoom> RoomByBotObjectId => _roomByBotObjectId;

        private readonly BotsConfigsProviderService _botsConfigsProviderService;
        private readonly SetPlayerAnimationPlaceModelsProviderService _setPlayerAnimationPlaceModelsProviderService;
        private readonly EmotionsConfigProviderService _emotionsConfigProviderService;
        private readonly IRoomsProviderService _roomsProviderService;
        private readonly ServerSetPlayersAnimationsPlacesSynchronizer _serverSetPlayersAnimationsPlacesSynchronizer;
        private readonly BotsNicknamesProviderService _botsNicknamesProviderService;
        private readonly BotsCustomizationDataProviderService _botsCustomizationDataProviderService;

        private readonly Dictionary<IRoom, HashSet<BotController>> _botBehaviourControllersByRoom =
            new Dictionary<IRoom, HashSet<BotController>>();

        private readonly Dictionary<int, IRoom> _roomByBotObjectId = new Dictionary<int, IRoom>();

        private readonly ClientsConfigsProviderService _clientsConfigsProviderService;
        private readonly ItemsConfigsProviderService _itemsConfigsProviderService;
        private readonly BotsPetsDataProviderService _botsPetsDataProviderService;
        private readonly PetsConfigProviderService _petsConfigsProviderService;

        private readonly Container _container = new Container();

        public const string BotAssetId =
#if ADDRESSABLES
            "Bot";
#else
            "WelwiseHubBotsModule/Runtime/Shared/Loadable/Bot";
#endif

        private const float MinimalValue = 0.1f;

        public BotsFactory(BotsConfigsProviderService botsConfigsProviderService,
            SetPlayerAnimationPlaceModelsProviderService setPlayerAnimationPlaceModelsProviderService,
            IRoomsProviderService roomsProviderService,
            EmotionsConfigProviderService emotionsConfigProviderService,
            ServerSetPlayersAnimationsPlacesSynchronizer serverSetPlayersAnimationsPlacesSynchronizer,
            BotsNicknamesProviderService botsNicknamesProviderService,
            BotsCustomizationDataProviderService botsCustomizationDataProviderService,
            ClientsConfigsProviderService clientsConfigsProviderService,
            ItemsConfigsProviderService itemsConfigsProviderService,
            BotsPetsDataProviderService botsPetsDataProviderService,
            PetsConfigProviderService petsConfigsProviderService)
        {
            _botsConfigsProviderService = botsConfigsProviderService;
            _setPlayerAnimationPlaceModelsProviderService = setPlayerAnimationPlaceModelsProviderService;
            _roomsProviderService = roomsProviderService;
            _emotionsConfigProviderService = emotionsConfigProviderService;
            _serverSetPlayersAnimationsPlacesSynchronizer = serverSetPlayersAnimationsPlacesSynchronizer;
            _botsNicknamesProviderService = botsNicknamesProviderService;
            _botsCustomizationDataProviderService = botsCustomizationDataProviderService;
            _clientsConfigsProviderService = clientsConfigsProviderService;
            _itemsConfigsProviderService = itemsConfigsProviderService;
            _botsPetsDataProviderService = botsPetsDataProviderService;
            _petsConfigsProviderService = petsConfigsProviderService;
        }

        public async UniTask<BotController> GetInitializedBotControllerAsync(IRoom room,
            Transform[] portalsTransforms, Transform shopTransform, Scene scene, IAssetLoader assetLoader,
            Action enteredPortal)
        {
            var prefab =
                await _container.GetOrLoadAndRegisterObjectAsync<SharedBotSerializableComponents>(BotAssetId,
                    assetLoader,
                    shouldCreate: false);

            var botsConfig = await _botsConfigsProviderService.GetBotsConfigAsync();

            var serializableComponents = Object.Instantiate(prefab, botsConfig.SpawnPosition, Quaternion.identity);

            InstanceFinder.ServerManager.Spawn(serializableComponents.gameObject, null, scene);

            var botModel = new BotModel(botsConfig,
                new Timer(serializableComponents.destroyCancellationToken),
                new Timer(serializableComponents.destroyCancellationToken));

            var objectId = serializableComponents.GetComponent<NetworkObject>().ObjectId;

            var portalsInteractPointGroupInteractor =
                new PortalsInteractPointGroupInteractor(serializableComponents, MinimalValue, portalsTransforms);

            var interestPointGroupDataProviders = GetInterestPointGroupInteractor(room, shopTransform,
                portalsInteractPointGroupInteractor, serializableComponents, botModel, objectId, botsConfig);

            var botBehaviourController = new BotController(serializableComponents, botModel,
                _emotionsConfigProviderService, interestPointGroupDataProviders);

            var heroAnimatorController =
                new HeroAnimatorController(serializableComponents.HeroAnimatorSerializableComponents);

            new BotAnimatorController(heroAnimatorController, botBehaviourController);

            portalsInteractPointGroupInteractor.EnteredPortal +=
                () =>
                {
                    InstanceFinder.ServerManager.Despawn(serializableComponents.gameObject);
                    enteredPortal?.Invoke();
                };

            var botObjectId = serializableComponents.GetComponent<NetworkObject>().ObjectId;

            CreatedBotBehaviourController?.Invoke(botBehaviourController, room);

            foreach (var connection in room.ConnectedClientsNetworkConnections)
            {
                if (connection.Scenes.Contains(scene))
                    InstanceFinder.ServerManager.Broadcast(
                        connection, new InitializationBotBroadcast(serializableComponents.gameObject,
                            _botsNicknamesProviderService.Nicknames[botObjectId],
                            _botsCustomizationDataProviderService.BotsCustomizationData[botObjectId],
                            _botsPetsDataProviderService.PetsData[botObjectId]));
            }

            if (!_botBehaviourControllersByRoom.ContainsKey(room))
            {
                _botBehaviourControllersByRoom.Add(room, new HashSet<BotController>());

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

        private Dictionary<InterestPointGroup, IInterestPointGroupInteractor> GetInterestPointGroupInteractor(
            IRoom room, Transform shopTransform,
            PortalsInteractPointGroupInteractor portalsInteractPointGroupInteractor,
            SharedBotSerializableComponents serializableComponents, BotModel botModel, int objectId,
            BotsConfig botsConfig)
        {
            var interestPointGroupDataProviders = new Dictionary<InterestPointGroup, IInterestPointGroupInteractor>()
            {
                {
                    InterestPointGroup.Portals,
                    portalsInteractPointGroupInteractor
                },
                {
                    InterestPointGroup.Shop,
                    new ShopInterestPointGroupInteractor(_itemsConfigsProviderService, _botsPetsDataProviderService,
                        _petsConfigsProviderService, _botsNicknamesProviderService,
                        _botsCustomizationDataProviderService, _clientsConfigsProviderService, serializableComponents,
                        MinimalValue, shopTransform, botModel, objectId, botsConfig)
                },
                {
                    InterestPointGroup.Bar,
                    new BarInterestPointGroupInteractor(serializableComponents,
                        _setPlayerAnimationPlaceModelsProviderService, _serverSetPlayersAnimationsPlacesSynchronizer,
                        room, objectId, MinimalValue)
                }
            };
            return interestPointGroupDataProviders;
        }
    }
}