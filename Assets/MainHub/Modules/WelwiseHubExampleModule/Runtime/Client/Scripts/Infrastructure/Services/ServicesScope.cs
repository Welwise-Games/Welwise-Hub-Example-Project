using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting.Bayou;
using Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.Network;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Client.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;
using WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud;
using WelwiseChatModule.Runtime.Client.Scripts.Network;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseCurrenciesModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Circle;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseGamesSDK;
using WelwiseGamesSDK.Shared;
using WelwiseHubBotsModule.Runtime.Client.Scripts;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.EmotionsSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.PetsSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SetEmotions;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.UI;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem.SetEmotions;
using WelwiseItemInShopModule.Client.Scripts;
using WelwiseItemInShopModule.Client.Scripts.Network;
using WelwisePetsModule.Runtime.Client.Scripts;
using WelwisePetsModule.Runtime.Client.Scripts.SetPet;
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.Services
{
    public sealed class ServicesScope : MonoBehaviour
    {
        [SerializeField] private Bayou _bayou;

        public async void Awake()
        {
#if UNITY_EDITOR
            _bayou.SetUseWSS(false);
#else
            _bayou.SetUseWSS(true);
#endif
            DontDestroyOnLoad(gameObject);

            var sdk = WelwiseSDK.Construct().AsTransient();
            await sdk.InitializeAsync();

            var clientManager = InstanceFinder.ClientManager;
            var assetLoader = AssetsLoaderTools.GetAssetLoader();

            new SharedSceneManagementService(InstanceFinder.SceneManager, clientManager);

            var clientsConfigsProviderService = new ClientsConfigsProviderService(assetLoader);
            var itemsConfigsProviderService = new ItemsConfigsProviderService(assetLoader);
            var itemsViewConfigsProviderService = new ItemsViewConfigsProviderService(assetLoader);

            var clientsDataProviderService = new ClientsDataProviderService();

            var clientsCustomizationDataProviderService = new ClientsCustomizationDataProviderService(
                new SharedClientsCustomizationDataProviderService(clientsDataProviderService,
                    await clientsConfigsProviderService.GetClientsConfigAsync(),
                    await itemsConfigsProviderService.GetItemsConfigAsync()), clientsDataProviderService);

            var sharedNicknamesConfigsProviderService = new SharedNicknamesConfigsProviderService(assetLoader);

            var uiFactory = new UIFactory(assetLoader);
            var mobileHudFactory = new MobileHudFactory();

            var sharedClientsNicknamesConfig =
                await sharedNicknamesConfigsProviderService.GetSharedClientsNicknamesConfigAsync();

            NicknameChangingTools.Initialize(clientManager, clientsDataProviderService,
                sdk.PlayerData, sharedClientsNicknamesConfig, out var nicknamesEntryPointData);

            var cameraFactory = new CameraFactory(assetLoader);
            var clothesFactory = new ClothesFactory(assetLoader);

            var shopUIFactory = new ShopUIFactory(clientsDataProviderService,
                nicknamesEntryPointData.ClientsNicknamesProviderService,
                sharedNicknamesConfigsProviderService,
                itemsConfigsProviderService, itemsViewConfigsProviderService, uiFactory, assetLoader);

            var connectionTrackingService = new ClientsConnectionTrackingServiceForClient(clientManager);

            var chatEntryPointDataContainer = new DataContainer<ChatEntryPointData>();

            await ChatEntryPointTools.InitializeAsync(clientManager, connectionTrackingService,
                clientsDataProviderService, chatEntryPointDataContainer, assetLoader);

            TrainingTools.Initialize(connectionTrackingService, assetLoader, out var trainingEntryPointData);

            var eventBus = new EventBus();

            var heroAudioClipsProviderService = new HeroAudioClipsProviderService(assetLoader);

            var inputConfigProviderService = new InputConfigProviderService(assetLoader);

            var inputService = await GetInputServiceAsync(inputConfigProviderService);
            inputConfigProviderService.Dispose();

            var loadingUIFactory = new LoadingUIFactory(eventBus, assetLoader);

            var clientConfigsProviderService = new ClientConfigsProviderService(assetLoader);

            var customCoroutineRunner = new GameObject().AddComponent<CustomCoroutineRunner>();

            DontDestroyOnLoad(customCoroutineRunner);

            var animationChangingViewConfigsProviderService =
                new AnimationChangingViewConfigsProviderService(assetLoader);

            var enteredToPortalEventProvider = new EnteredToPortalEventProvider();

            var emotionsConfigsProviderService = new EmotionsConfigProviderService(assetLoader);
            var emotionsViewConfigsProviderService = new EmotionsViewConfigProviderService(assetLoader);

            #region PetsModuleInitialization

            var petsConfigProviderService = new PetsConfigProviderService(assetLoader);
            var petsViewConfigsProviderService = new PetsViewConfigProviderService(assetLoader);

            var petsViewFactory = new PetsViewFactory(petsViewConfigsProviderService, assetLoader);

            var playersFactory = new PlayersFactory(cameraFactory,
                chatEntryPointDataContainer.Data.ChatFactory,
                clientsCustomizationDataProviderService, clientsDataProviderService, clothesFactory,
                emotionsViewConfigsProviderService, nicknamesEntryPointData.ClientsNicknamesProviderService,
                eventBus, heroAudioClipsProviderService, inputService, assetLoader, itemsViewConfigsProviderService,
                petsViewFactory);

            var playersPetsViewControllersProviderService =
                new PlayersPetsViewControllerProviderService(playersFactory);

            var petsEntryPointDataContainer = new DataContainer<PetsEntryPointData>();
            await PetsEntryPointTools.InitializeAsync(petsEntryPointDataContainer,
                sdk.PlayerData, assetLoader, clientManager, playersPetsViewControllersProviderService, petsViewFactory,
                petsConfigProviderService, petsViewConfigsProviderService);

            var setPetsUIFactory =
                new SetPetsUIFactory(petsConfigProviderService, petsViewConfigsProviderService,
                    petsEntryPointDataContainer.Data.OwnerSelectedPetsDataProviderService, assetLoader);

            var ownerPetsSetSynchronizer =
                new OwnerItemsSetSynchronizerService<SelectedPetData, SelectedPetsData,
                    SetSelectedPetsBroadcastForServer>(
                    petsEntryPointDataContainer.Data.OwnerSelectedPetsDataProviderService, clientManager,
                    setPetsUIFactory.SetItemsUIFactory,
                    data => new SetSelectedPetsBroadcastForServer(data));

            #endregion

            var notOwnerPlayersComponentsProviderService =
                new NotOwnerPlayersComponentsProviderService(playersFactory);

            #region EmotionsModuleInitialization

            var emotionsEntryPointDataContainer = new DataContainer<EmotionsEntryPointData>();

            await EmotionsEntryPointTools.InitializeAsync(emotionsEntryPointDataContainer,
                notOwnerPlayersComponentsProviderService, clientManager,
                sdk.PlayerData,
                assetLoader,
                emotionsConfigsProviderService, emotionsViewConfigsProviderService);

            var setEmotionsUIFactory =
                new SetEmotionsUIFactory(emotionsConfigsProviderService, emotionsViewConfigsProviderService,
                    emotionsEntryPointDataContainer.Data.OwnerSelectedEmotionsDataProviderService, assetLoader);

            var ownerEmotionsSetSynchronizer =
                new OwnerItemsSetSynchronizerService<SelectedEmotionData, SelectedEmotionsData,
                    SetSelectedEmotionsBroadcastForServer>(
                    emotionsEntryPointDataContainer.Data.OwnerSelectedEmotionsDataProviderService, clientManager,
                    setEmotionsUIFactory.SetItemsUIFactory,
                    data => new SetSelectedEmotionsBroadcastForServer(data));

            #endregion

            var currenciesProviderService = new CurrenciesProviderService();

            BotsEntryPointTools.Initialize(cameraFactory, clientManager, connectionTrackingService,
                emotionsConfigsProviderService, emotionsViewConfigsProviderService,
                emotionsEntryPointDataContainer.Data.EmotionsViewFactory, itemsViewConfigsProviderService,
                clothesFactory,
                assetLoader, petsEntryPointDataContainer.Data.BotsPetsDataProviderService,
                petsViewFactory, eventBus);

            ChangingAnimationsTools.Initialize(
                () => playersFactory.OwnerPlayerComponents?.SerializableComponents.transform,
                eventBus, clientManager, connectionTrackingService, out var changingAnimationsDataFromInitialize,
                customCoroutineRunner);

            MiniGamesEntryPointTools.Initialize(out var miniGamesEntryPointData, assetLoader);

            var hubFactory = new HubFactory(shopUIFactory, playersFactory, clientsDataProviderService,
                clothesFactory,
                clientsCustomizationDataProviderService, emotionsConfigsProviderService,
                emotionsEntryPointDataContainer.Data.EmotionsViewFactory, itemsConfigsProviderService,
                eventBus, clientConfigsProviderService, cameraFactory, animationChangingViewConfigsProviderService,
                changingAnimationsDataFromInitialize.SetPlayerAnimationButtonControllersProviderService,
                enteredToPortalEventProvider, inputService, mobileHudFactory, uiFactory,
                emotionsViewConfigsProviderService, assetLoader, itemsViewConfigsProviderService,
                petsEntryPointDataContainer.Data.PetsViewFactory, miniGamesEntryPointData.MiniGamesFactory,
                miniGamesEntryPointData.MiniGamesConfigProviderService, currenciesProviderService);

            new SubscribingMediator(playersFactory, hubFactory, connectionTrackingService,
                emotionsEntryPointDataContainer.Data.OwnerEmotionsPlayingSynchronizerService, eventBus, shopUIFactory,
                clientsDataProviderService, clientsCustomizationDataProviderService,
                chatEntryPointDataContainer.Data.ChatsDataProviderService,
                clientManager, sdk.Environment, sdk, nicknamesEntryPointData.ClientsNicknamesProviderService,
                chatEntryPointDataContainer.Data.ChatFactory, currenciesProviderService);

            RegisterGameStateMachine(cameraFactory, shopUIFactory, chatEntryPointDataContainer.Data.ChatFactory,
                eventBus,
                playersFactory,
                emotionsEntryPointDataContainer.Data.EmotionsCircleFactory, setEmotionsUIFactory, hubFactory,
                loadingUIFactory,
                clientManager, enteredToPortalEventProvider, sdk,
                trainingEntryPointData.TrainingFactory, connectionTrackingService, inputService, uiFactory,
                mobileHudFactory, assetLoader, setPetsUIFactory, miniGamesEntryPointData.MiniGamesFactory);
        }

        private void RegisterGameStateMachine(CameraFactory cameraFactory, ShopUIFactory shopUIFactory,
            ChatFactory chatFactory, EventBus eventBus, PlayersFactory playersFactory,
            EmotionsCircleFactory emotionsCircleFactory, SetEmotionsUIFactory setEmotionsUIFactory,
            HubFactory hubFactory, LoadingUIFactory loadingUIFactory,
            ClientManager clientManager,
            EnteredToPortalEventProvider enteredToPortalEventProvider, ISDK sdk, TrainingFactory trainingFactory,
            ClientsConnectionTrackingServiceForClient clientsConnectionTrackingService, IInputService inputService,
            UIFactory uiFactory, MobileHudFactory mobileHudFactory, IAssetLoader assetLoader,
            SetPetsUIFactory setPetsUIFactory, MiniGamesFactory miniGamesFactory)
            => new GameStateMachine(new BootstrapGameState(cameraFactory, loadingUIFactory),
                new InitializationGameState(clientManager),
                new HubGameState(shopUIFactory, chatFactory, playersFactory, emotionsCircleFactory,
                    setEmotionsUIFactory, loadingUIFactory, hubFactory, sdk,
                    enteredToPortalEventProvider, trainingFactory, clientsConnectionTrackingService,
                    uiFactory, assetLoader, setPetsUIFactory),
                new ReconnectionGameState(shopUIFactory, hubFactory, emotionsCircleFactory, chatFactory,
                    loadingUIFactory,
                    playersFactory, inputService, uiFactory, mobileHudFactory, setEmotionsUIFactory, setPetsUIFactory, miniGamesFactory),
                eventBus, assetLoader);

        private async UniTask<IInputService>
            GetInputServiceAsync(InputConfigProviderService inputConfigProviderService) =>
            DeviceDetectorTools.IsMobile()
                ? new MobileInputService()
                : new DekstopInputService(await inputConfigProviderService.GetInputConfigAsync());
    }
}