using System.Linq;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Managing.Scened;
using FishNet.Managing.Server;
using FishNet.Transporting.Bayou;
using Modules.WelwiseChatModule.Runtime.Server.Scripts;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Server.Scripts;
using WelwiseChangingNicknameModule.Runtime.Server.Scripts.Services;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseHubBotsModule.Runtime.Server.Scripts;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.GameStateMachinePart;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.Services.Data;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.ChatSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.EmotionsSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.PlayerSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.ShopSystem.SettingEmotions;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Holders;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.Services
{
    public sealed class ServicesScope : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private Bayou _bayou;
        [SerializeField] private int _playerLimitPerHub;

        public async void Awake()
        {
#if UNITY_EDITOR
            _bayou.SetUseWSS(false);
#else
            _bayou.SetUseWSS(true);
#endif

            DontDestroyOnLoad(gameObject);

            var assetLoader = AssetsLoaderTools.GetAssetLoader();

            var clientsConfigsProviderService = new ClientsConfigsProviderService(assetLoader);
            var itemsConfigsProviderService = new ItemsConfigsProviderService(assetLoader);
            var serverManager = InstanceFinder.ServerManager;
            var sceneManager = InstanceFinder.SceneManager;

            RegisterServices(out var clientsConnectionTrackingServiceForServer, out var eventBus,
                out var hubsProviderService, out var serverSceneManagementService,
                out var clientsNetworkConnectionsProviderService, out var serverChatsDataProvider,
                out var visibleClientsProviderService,
                out var clientsDataProviderService, 
                out var clientsCustomizationDataProviderService, out var emotionsConfigsProviderService,
                out var setPlayerAnimationPlaceModelsProviderService,
                out var botsEntryPointData, out var playersConfigsProviderService,
                await clientsConfigsProviderService.GetClientsConfigAsync(),
                await itemsConfigsProviderService.GetItemsConfigAsync(),
                serverManager, sceneManager, InstanceFinder.ClientManager, itemsConfigsProviderService,
                clientsConfigsProviderService, assetLoader);

            RegisterFactories(out var playerFactory, playersConfigsProviderService, serverManager, assetLoader);

            EmotionsEntryPointTools.Initialize(serverManager, visibleClientsProviderService,
                emotionsConfigsProviderService, out var emotionsEntryPointData);

            var sharedNicknamesConfigsProviderService = new SharedNicknamesConfigsProviderService(assetLoader);

            var sharedClientsNicknamesConfig =
                await sharedNicknamesConfigsProviderService.GetSharedClientsNicknamesConfigAsync();

            NicknamesSharedEntryPointTools.Initialize(sharedClientsNicknamesConfig, serverManager,
                visibleClientsProviderService,
                clientsDataProviderService, out var nicknamesEntryPointData);

            new SubscribingMediator(clientsConnectionTrackingServiceForServer, hubsProviderService,
                playerFactory, serverSceneManagementService, serverChatsDataProvider,
                emotionsEntryPointData.ClientsSelectedEmotionsDataProviderService, clientsDataProviderService, eventBus,
                clientsCustomizationDataProviderService,
                serverManager, sceneManager, botsEntryPointData.BotsFactory,
                await botsEntryPointData.BotsConfigsProviderService.GetBotsConfigAsync(), assetLoader);

            ChatEntryPointTools.Initialize(serverChatsDataProvider, clientsNetworkConnectionsProviderService,
                clientsDataProviderService, serverManager);

            new ServerEmotionsSettingSynchronizerService(serverManager,
                emotionsEntryPointData.ClientsSelectedEmotionsDataProviderService);

            InitializeGameStateMachine(clientsConnectionTrackingServiceForServer, eventBus, hubsProviderService,
                playerFactory, serverChatsDataProvider,
                clientsDataProviderService, clientsConfigsProviderService,
                serverManager, emotionsConfigsProviderService, assetLoader);
        }

        private void RegisterFactories(out PlayersFactory playersFactory,
            PlayersConfigsProviderService playersConfigsProviderService, ServerManager serverManager, IAssetLoader assetLoader) =>
            playersFactory = new PlayersFactory(serverManager, playersConfigsProviderService, assetLoader);

        private void RegisterServices(
            out ClientsConnectionTrackingServiceForServer clientsConnectionTrackingServiceForServer,
            out EventBus eventBus, out HubsProviderService hubsProviderService,
            out ServerSceneManagementService sceneManagementService,
            out ClientsNetworkConnectionsProviderService clientsNetworkConnectionsProviderService,
            out ServerChatsDataProvider serverChatsDataProvider,
            out IVisibleClientsProviderService visibleClientsProviderService,
            out ClientsDataProviderService clientsDataProviderService,
            out ClientsCustomizationDataProviderService clientsCustomizationDataProviderService,
            out EmotionsConfigsProviderService emotionsConfigsProviderService,
            out SetPlayerAnimationPlaceModelsProviderService setPlayerAnimationPlaceModelsProviderService,
            out BotsEntryPointData botsEntryPointData, out PlayersConfigsProviderService playersConfigsProviderService,
            ClientsConfig clientsConfig, ItemsConfig itemsConfig, ServerManager serverManager,
            SceneManager sceneManager,
            ClientManager clientManager, ItemsConfigsProviderService itemsConfigsProviderService,
            ClientsConfigsProviderService clientsConfigsProviderService, IAssetLoader assetLoader)
        {
            clientsConnectionTrackingServiceForServer = new ClientsConnectionTrackingServiceForServer();
            new SharedSceneManagementService(sceneManager, clientManager);

            setPlayerAnimationPlaceModelsProviderService = new SetPlayerAnimationPlaceModelsProviderService();

            hubsProviderService = new HubsProviderService(serverManager, sceneManager, assetLoader, _playerLimitPerHub);

            sceneManagementService = new ServerSceneManagementService();
            emotionsConfigsProviderService = new EmotionsConfigsProviderService(assetLoader);

            ChangingAnimationsTools.Initialize(hubsProviderService,
                room => Enumerable.Range(0, ((Hub)room).Instance.SetPlayerAnimationPlacesTransforms.Length)
                    .Select(i =>
                        new SetPlayerAnimationPlaceModel(i,
                            ((Hub)room).Instance.SetPlayerAnimationPlacesTransforms[i].position)).ToHashSet(),
                serverManager,
                out var changingAnimationEntryPointData, setPlayerAnimationPlaceModelsProviderService);

            BotsEntryPointTools.Initialize(setPlayerAnimationPlaceModelsProviderService,
                sceneManagementService, hubsProviderService, serverManager, ScenesNames.Hub,
                emotionsConfigsProviderService, out botsEntryPointData,
                changingAnimationEntryPointData.ServerSetPlayersAnimationsPlacesSynchronizer,
                clientsConfigsProviderService, itemsConfigsProviderService, assetLoader);

            eventBus = new EventBus();
            clientsNetworkConnectionsProviderService =
                new ClientsNetworkConnectionsProviderService(hubsProviderService);
            serverChatsDataProvider = new ServerChatsDataProvider(hubsProviderService);
            visibleClientsProviderService = new VisibleClientsProviderService(hubsProviderService);
            clientsDataProviderService = new ClientsDataProviderService();
            clientsCustomizationDataProviderService = new ClientsCustomizationDataProviderService(
                new SharedClientsCustomizationDataProviderService(clientsDataProviderService, clientsConfig,
                    itemsConfig));

            playersConfigsProviderService = new PlayersConfigsProviderService(assetLoader);
        }

        private void InitializeGameStateMachine(
            ClientsConnectionTrackingServiceForServer clientsConnectionTrackingServiceForServer,
            EventBus eventBus, HubsProviderService hubsProviderService, PlayersFactory playersFactory,
            ServerChatsDataProvider serverChatsDataProvider, ClientsDataProviderService clientsDataProviderService,
            ClientsConfigsProviderService clientsConfigsProviderService, ServerManager serverManager,
            EmotionsConfigsProviderService emotionsConfigsProviderService, IAssetLoader assetLoader)
        {
            new GameStateMachine(new ClientInitializationGameState(eventBus, clientsDataProviderService,
                    clientsConfigsProviderService, emotionsConfigsProviderService),
                new HubGameState(playersFactory, hubsProviderService, serverChatsDataProvider, serverManager), eventBus,
                _networkManager, clientsConnectionTrackingServiceForServer, assetLoader);
        }
    }
}