using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.Network;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;
using WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.Services;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.UI;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using ClientConfigsProviderService =
    WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.Services.ClientConfigsProviderService;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class HubFactory
    {
        public ClientHubComponents ClientHubComponents { get; private set; }

        private readonly ShopUIFactory _shopUIFactory;
        private readonly PlayersFactory _playersFactory;
        private readonly ItemsConfigsProviderService _itemsConfigsProviderService;
        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly ClothesFactory _clothesFactory;
        private readonly ClientsCustomizationDataProviderService _clientsCustomizationDataProviderService;
        private readonly EmotionsConfigsProviderService _emotionsConfigsProviderService;
        private readonly EmotionsViewFactory _emotionsViewFactory;
        private readonly EventBus _eventBus;
        private readonly ClientConfigsProviderService _clientConfigsProviderService;
        private readonly AnimationChangingViewConfigsProviderService _animationChangingViewConfigsProviderService;

        private readonly EnteredToPortalEventProvider _enteredToPortalEventProvider;

        private readonly EmotionsViewConfigsProviderService _emotionsViewConfigsProviderService;

        private readonly SetPlayerAnimationButtonControllersProviderService
            _setPlayerAnimationButtonControllersProviderService;

        private readonly CameraFactory _cameraFactory;
        private readonly IInputService _inputService;

        private readonly MobileHudFactory _mobileHudFactory;
        private readonly UIFactory _uiFactory;
        private readonly IAssetLoader _assetLoader;
        private readonly ItemsViewConfigsProviderService _itemsViewConfigsProviderService;

        private readonly Container _container = new Container();

        private const string HubViewAssetId =
#if ADDRESSABLES
            "HubView";
#else
            "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/HubView";
#endif

        public HubFactory(ShopUIFactory shopUIFactory, PlayersFactory playersFactory,
            ClientsDataProviderService clientsDataProviderService,
            ClothesFactory clothesFactory,
            ClientsCustomizationDataProviderService clientsCustomizationDataProviderService,
            EmotionsConfigsProviderService emotionsConfigsProviderService, EmotionsViewFactory emotionsViewFactory,
            ItemsConfigsProviderService itemsConfigsProviderService, EventBus eventBus,
            ClientConfigsProviderService clientConfigsProviderService, CameraFactory cameraFactory,
            AnimationChangingViewConfigsProviderService animationChangingViewConfigsProviderService,
            SetPlayerAnimationButtonControllersProviderService setPlayerAnimationButtonControllersProviderService,
            EnteredToPortalEventProvider enteredToPortalEventProvider, IInputService inputService,
            MobileHudFactory mobileHudFactory, UIFactory uiFactory,
            EmotionsViewConfigsProviderService emotionsViewConfigsProviderService, IAssetLoader assetLoader,
            ItemsViewConfigsProviderService itemsViewConfigsProviderService)
        {
            _shopUIFactory = shopUIFactory;
            _playersFactory = playersFactory;
            _clientsDataProviderService = clientsDataProviderService;
            _clothesFactory = clothesFactory;
            _clientsCustomizationDataProviderService = clientsCustomizationDataProviderService;
            _emotionsConfigsProviderService = emotionsConfigsProviderService;
            _emotionsViewFactory = emotionsViewFactory;
            _itemsConfigsProviderService = itemsConfigsProviderService;
            _eventBus = eventBus;
            _clientConfigsProviderService = clientConfigsProviderService;
            _cameraFactory = cameraFactory;
            _animationChangingViewConfigsProviderService = animationChangingViewConfigsProviderService;
            _setPlayerAnimationButtonControllersProviderService = setPlayerAnimationButtonControllersProviderService;
            _enteredToPortalEventProvider = enteredToPortalEventProvider;
            _inputService = inputService;
            _mobileHudFactory = mobileHudFactory;
            _uiFactory = uiFactory;
            _emotionsViewConfigsProviderService = emotionsViewConfigsProviderService;
            _assetLoader = assetLoader;
            _itemsViewConfigsProviderService = itemsViewConfigsProviderService;
        }

        public async UniTask DisposeAsync() => await _container.DestroyAndClearAllImplementationsAsync();

        public async UniTask<ClientHubComponents> GetInitializedHubAndShopAsync(GameObject hubInstance,
            IPlatformNavigation platformNavigation)
        {
            await AsyncTools.WaitWhileWithoutSkippingFrame(() => _playersFactory.OwnerPlayerComponents == null);

            var hubSerializableComponents = await
                _container.GetOrLoadAndRegisterObjectAsync<ClientHubSerializableComponents>(HubViewAssetId,
                    _assetLoader,
                    parent: hubInstance.transform);

            CreatePortalsControllers(platformNavigation, hubSerializableComponents, _enteredToPortalEventProvider);

            ChangingAnimationsTools.CreateAndAppointSetPlayerAnimationAndPositionButtonControllers(
                await _cameraFactory.GetMainCameraAsync(),
                hubSerializableComponents.SetPlayerAnimationAndPositionAndRotationButtonsSerializableComponents,
                await _animationChangingViewConfigsProviderService.GetSetPlayerAnimationsAndPositionButtonsAsync(),
                _eventBus, _setPlayerAnimationButtonControllersProviderService);

            if (DeviceDetectorTools.IsMobile())
            {
                var mobileHudController = await _mobileHudFactory.GetCreatedMobileHudControllerAsync(
                    (await _uiFactory.GetUIRootAsync()).SerializableComponents.MobileHudSerializableComponents);

                if (_inputService is MobileInputService mobileInputService)
                    mobileInputService.Construct(mobileHudController);
            }

            await CreatePlayersAnimationsControllersAsync(hubSerializableComponents);

            var shopController = new ShopController(
                hubSerializableComponents.ShopSerializableComponents, _clientsDataProviderService.Data.GetOwners(),
                await _itemsViewConfigsProviderService.GetItemsViewConfigAsync(), _clothesFactory,
                _playersFactory.OwnerPlayerComponents.CharacterComponents.MovementController);

            var shopSettingEquippedItemsModel = new ShopSettingEquippedItemsModel(
                await _itemsConfigsProviderService.GetItemsConfigAsync(), _clientsDataProviderService,
                _clientsCustomizationDataProviderService);

            var shopPopupController =
                await _shopUIFactory.GetCreatedShopPopupControllerAsync(shopController, shopSettingEquippedItemsModel);

            shopPopupController.ShopPopup.Popup.Closed += shopController.StopPreview;
            shopController.StartedPreview += shopPopupController.ShopPopup.Popup.TryOpening;

            new PreviewRotatorController(hubSerializableComponents.ShopSerializableComponents.PlayerPreviewTransform,
                shopPopupController.ShopPopup.PlayerViewRawImagePointerUpDownObserver,
                shopPopupController.ShopPopup.PlayerViewRawImagePointerDragObserver,
                hubSerializableComponents.ShopSerializableComponents.PlayerPreviewRotationSensitivity);

            return ClientHubComponents = new ClientHubComponents(hubSerializableComponents, shopController);
        }

        private void CreatePortalsControllers(IPlatformNavigation platformNavigation,
            ClientHubSerializableComponents clientHubSerializableComponents,
            EnteredToPortalEventProvider enteredToPortalEventProvider)
        {
            Enumerable.Range(0, clientHubSerializableComponents.PortalsSerializableComponents.Length).ForEach(async
                index =>
            {
                var portalController = new PortalController(
                    clientHubSerializableComponents.PortalsSerializableComponents[index], index,
                    await _clientConfigsProviderService.GetPortalsConfigAsync(),
                    platformNavigation, async error =>
                        (await _uiFactory.GetUIRootAsync()).ErrorTextController.SetTextAndStartAnimationAsync(
                            LocalizationKeysHolder.GameIsNotAvailableNow, LocalizationTablesHolder.Portals));

                portalController.OwnerEnteredToPortal += enteredToPortalEventProvider.InvokeOwnerEnteredToPortal;
            });
        }

        private async UniTask CreatePlayersAnimationsControllersAsync(
            ClientHubSerializableComponents clientHubSerializableComponents)
        {
            foreach (var components in clientHubSerializableComponents.AnimatedWithEmotionHeroSerializableComponents)
            {
                var animatorStateObserver = components.PlayerEmotionsSerializableComponents.Animator.gameObject
                    .GetOrAddComponent<AnimatorStateObserver>();

                var emotionsAnimatorController = new EmotionsAnimatorController(
                    components.PlayerEmotionsSerializableComponents.Animator,
                    animatorStateObserver,
                    components.PlayerEmotionsSerializableComponents.Animator.gameObject
                        .GetOrAddComponent<ParticleEventController>(),
                    await _emotionsViewConfigsProviderService.GetEmotionsViewConfigAsync());

                var heroEmotionsAnimatorController = new StandEmotionsAnimatorController(
                    emotionsAnimatorController, components.PlayerEmotionsSerializableComponents.Animator,
                    animatorStateObserver);

                heroEmotionsAnimatorController.EndedAnimation += () =>
                    PlayEmotionAnimationAsyncAndUpdateParticles(components, emotionsAnimatorController);

                PlayEmotionAnimationAsyncAndUpdateParticles(components, emotionsAnimatorController);
            }
        }

        async void PlayEmotionAnimationAsyncAndUpdateParticles(AnimatedWithEmotionHeroSerializableComponents components,
            EmotionsAnimatorController emotionsAnimatorController)
        {
            emotionsAnimatorController.SetAnimatorControllerAndTryStartingEmotionAnimation(
                (await _emotionsConfigsProviderService.GetEmotionsAnimationsConfig()).EmotionsAnimationConfigs
                .FirstOrDefault(config => config.EmotionIndex == components.EmotionIndex)
                ?.OverrideController, components.EmotionIndex, -1);

            var particlesParents = await _emotionsViewFactory.TryCreatingParticlesParentsAsync(
                emotionsAnimatorController.ParticleEventController.transform,
                components.EmotionIndex);

            particlesParents.ForEach(parent =>
                parent.OptionalParticles.ForEach(particle => particle.gameObject.SetActive(false)));

            emotionsAnimatorController.ParticleEventController.UpdateParticleObjects(
                particlesParents.Select(parent => parent.gameObject).ToArray());
        }
    }
}