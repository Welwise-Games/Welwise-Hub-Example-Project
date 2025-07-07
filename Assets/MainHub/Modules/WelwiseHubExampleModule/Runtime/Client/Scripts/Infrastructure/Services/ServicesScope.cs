using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting.Bayou;
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
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Circle;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseGamesSDK;
using WelwiseGamesSDK.Shared;
using WelwiseHubBotsModule.Runtime.Client.Scripts;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.EmotionsSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions.Network.Owner;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.UI;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
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

            var emotionsConfigsProviderService = new EmotionsConfigsProviderService(assetLoader);
            var emotionsViewConfigsProviderService = new EmotionsViewConfigsProviderService(assetLoader);

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

            var playersFactory = new PlayersFactory(cameraFactory,
                chatEntryPointDataContainer.Data.ChatFactory,
                clientsCustomizationDataProviderService, clientsDataProviderService, clothesFactory,
                emotionsViewConfigsProviderService, nicknamesEntryPointData.ClientsNicknamesProviderService,
                eventBus, heroAudioClipsProviderService, inputService, assetLoader, itemsViewConfigsProviderService);

            var loadingUIFactory = new LoadingUIFactory(eventBus, assetLoader);

            var notOwnerPlayersComponentsProviderService =
                new NotOwnerPlayersComponentsProviderService(playersFactory);

            var emotionsAnimationsConfig = await emotionsConfigsProviderService.GetEmotionsAnimationsConfig();

            EmotionsEntryPointTools.Initialize(notOwnerPlayersComponentsProviderService,
                clientManager, emotionsAnimationsConfig, emotionsConfigsProviderService,
                emotionsViewConfigsProviderService,
                sdk.PlayerData, out var emotionsEntryPointData, assetLoader);

            var clientConfigsProviderService = new ClientConfigsProviderService(assetLoader);

            var customCoroutineRunner = new GameObject().AddComponent<CustomCoroutineRunner>();

            DontDestroyOnLoad(customCoroutineRunner);

            var animationChangingViewConfigsProviderService =
                new AnimationChangingViewConfigsProviderService(assetLoader);

            ChangingAnimationsTools.Initialize(
                () => playersFactory.OwnerPlayerComponents?.SerializableComponents.transform,
                eventBus, clientManager, connectionTrackingService, out var changingAnimationsDataFromInitialize,
                customCoroutineRunner);

            var enteredToPortalEventProvider = new EnteredToPortalEventProvider();

            var hubFactory = new HubFactory(shopUIFactory, playersFactory, clientsDataProviderService,
                clothesFactory,
                clientsCustomizationDataProviderService, emotionsConfigsProviderService,
                emotionsEntryPointData.EmotionsViewFactory, itemsConfigsProviderService,
                eventBus, clientConfigsProviderService, cameraFactory, animationChangingViewConfigsProviderService,
                changingAnimationsDataFromInitialize.SetPlayerAnimationButtonControllersProviderService,
                enteredToPortalEventProvider, inputService, mobileHudFactory, uiFactory,
                emotionsViewConfigsProviderService, assetLoader, itemsViewConfigsProviderService);

            var settingEmotionsUIFactory =
                new SettingEmotionsUIFactory(emotionsConfigsProviderService, emotionsViewConfigsProviderService,
                    emotionsEntryPointData.OwnerSelectedEmotionsDataProviderService, assetLoader);

            BotsEntryPointTools.Initialize(cameraFactory, clientManager, connectionTrackingService,
                emotionsConfigsProviderService, emotionsViewConfigsProviderService,
                emotionsEntryPointData.EmotionsViewFactory, itemsViewConfigsProviderService, clothesFactory,
                assetLoader);

            var ownerEmotionsSettingSynchronizer =
                new OwnerEmotionsSettingSynchronizerService(
                    emotionsEntryPointData.OwnerSelectedEmotionsDataProviderService, clientManager,
                    settingEmotionsUIFactory);

            new SubscribingMediator(playersFactory, hubFactory, connectionTrackingService,
                emotionsEntryPointData.OwnerEmotionsPlayingSynchronizerService, eventBus, shopUIFactory,
                clientsDataProviderService, clientsCustomizationDataProviderService,
                chatEntryPointDataContainer.Data.ChatsDataProviderService,
                clientManager, sdk.Environment, sdk, nicknamesEntryPointData.ClientsNicknamesProviderService,
                chatEntryPointDataContainer.Data.ChatFactory);


            RegisterGameStateMachine(cameraFactory, shopUIFactory, chatEntryPointDataContainer.Data.ChatFactory,
                eventBus,
                playersFactory,
                emotionsEntryPointData.EmotionsCircleFactory, settingEmotionsUIFactory, hubFactory, loadingUIFactory,
                clientManager, enteredToPortalEventProvider, sdk,
                trainingEntryPointData.TrainingFactory, connectionTrackingService, inputService, uiFactory,
                mobileHudFactory, assetLoader);
        }

        private void RegisterGameStateMachine(CameraFactory cameraFactory, ShopUIFactory shopUIFactory,
            ChatFactory chatFactory, EventBus eventBus, PlayersFactory playersFactory,
            EmotionsCircleFactory emotionsCircleFactory, SettingEmotionsUIFactory settingEmotionsUIFactory,
            HubFactory hubFactory, LoadingUIFactory loadingUIFactory,
            ClientManager clientManager,
            EnteredToPortalEventProvider enteredToPortalEventProvider, ISDK sdk, TrainingFactory trainingFactory,
            ClientsConnectionTrackingServiceForClient clientsConnectionTrackingService, IInputService inputService,
            UIFactory uiFactory, MobileHudFactory mobileHudFactory, IAssetLoader assetLoader)
            => new GameStateMachine(new BootstrapGameState(cameraFactory, loadingUIFactory),
                new InitializationGameState(clientManager),
                new HubGameState(shopUIFactory, chatFactory, playersFactory, emotionsCircleFactory,
                    settingEmotionsUIFactory, loadingUIFactory, hubFactory, sdk,
                    enteredToPortalEventProvider, trainingFactory, clientsConnectionTrackingService,
                    uiFactory, assetLoader),
                new ReconnectionGameState(shopUIFactory, hubFactory, emotionsCircleFactory, chatFactory,
                    loadingUIFactory,
                    playersFactory, inputService, uiFactory, mobileHudFactory),
                eventBus, assetLoader);

        private async UniTask<IInputService>
            GetInputServiceAsync(InputConfigProviderService inputConfigProviderService) =>
            DeviceDetectorTools.IsMobile()
                ? new MobileInputService()
                : new DekstopInputService(await inputConfigProviderService.GetInputConfigAsync());
    }
}