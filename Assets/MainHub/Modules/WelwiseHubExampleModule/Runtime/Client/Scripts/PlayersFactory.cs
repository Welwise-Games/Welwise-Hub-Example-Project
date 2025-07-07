using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using MainHub.Modules.WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts;
using WelwiseChangingNicknameModule.Runtime.Client.Scripts.Services;
using WelwiseCharacterModule.Runtime.Client.Scripts;
using WelwiseCharacterModule.Runtime.Client.Scripts.HeroAnimators;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;
using WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud;
using WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement;
using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerCamera;
using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents;
using WelwiseCharacterModule.Runtime.Shared.Scripts;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.Services;
using WelwiseHubExampleModule.Runtime.Client.Scripts.UI;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts
{
    public class PlayersFactory
    {
        public IReadOnlyDictionary<NetworkConnection, ClientPlayerComponents> ClientsComponents => _clientsComponents;

        public event Action<OwnerPlayerComponents> OwnerPlayerInitialized;
        public event Action<NetworkConnection, ClientPlayerComponents> ClientPlayerInitialized;

        public OwnerPlayerComponents OwnerPlayerComponents { get; private set; }

        private readonly CameraFactory _cameraFactory;
        private readonly ChatFactory _chatFactory;
        private readonly ClientsCustomizationDataProviderService _clientsCustomizationDataProviderService;
        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly EmotionsViewConfigsProviderService _emotionsViewConfigsProviderService;
        private readonly ClientsNicknamesProviderService _clientsNicknamesProviderService;

        private readonly Dictionary<NetworkConnection, ClientPlayerComponents>
            _clientsComponents = new Dictionary<NetworkConnection, ClientPlayerComponents>();

        private readonly ItemsViewConfigsProviderService _itemsViewConfigsProviderService;
        private readonly ClothesFactory _clothesFactory;
        private readonly EventBus _eventBus;
        private readonly HeroAudioClipsProviderService _heroAudioClipsProviderService;
        private readonly IInputService _inputService;
        private readonly IAssetLoader _assetLoader;

        private readonly Container _container = new Container();

        private const string PlayerViewAssetId =
#if ADDRESSABLES
        "PlayerView";
#else
            "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/PlayerView";
#endif

        public PlayersFactory(CameraFactory cameraFactory, ChatFactory chatFactory,
            ClientsCustomizationDataProviderService clientsCustomizationDataProviderService,
            ClientsDataProviderService clientsDataProviderService,
            ClothesFactory clothesFactory, EmotionsViewConfigsProviderService emotionsViewConfigsProviderService,
            ClientsNicknamesProviderService clientsNicknamesProviderService,
            EventBus eventBus,
            HeroAudioClipsProviderService heroAudioClipsProviderService, IInputService inputService,
            IAssetLoader assetLoader, ItemsViewConfigsProviderService itemsViewConfigsProviderService)
        {
            _cameraFactory = cameraFactory;
            _chatFactory = chatFactory;
            _clientsCustomizationDataProviderService = clientsCustomizationDataProviderService;
            _clientsDataProviderService = clientsDataProviderService;
            _clothesFactory = clothesFactory;
            _emotionsViewConfigsProviderService = emotionsViewConfigsProviderService;
            _clientsNicknamesProviderService = clientsNicknamesProviderService;
            _eventBus = eventBus;
            _heroAudioClipsProviderService = heroAudioClipsProviderService;
            _inputService = inputService;
            _assetLoader = assetLoader;
            _itemsViewConfigsProviderService = itemsViewConfigsProviderService;
        }

        public void TryRemovingPlayer(NetworkConnection networkConnection)
        {
            if (networkConnection.IsOwners())
                _clientsComponents.Clear();
            else
                _clientsComponents.Remove(networkConnection);
        }

        public async UniTask<OwnerPlayerComponents> GetInitializedOwnerPlayerComponentsAsync(
            SharedPlayerSerializableComponents sharedSerializableComponents)
        {
            var serializableComponents = (await GetClientPlayerComponentsAsync(sharedSerializableComponents))
                .GetComponent<OwnerPlayerSerializableComponents>();

            var clientComponents = await GetInitializedClientPlayerComponents(SharedNetworkTools.OwnerConnection,
                serializableComponents.ClientSerializableComponents, sharedSerializableComponents);

            var cameraController = new CameraController(serializableComponents.transform,
                await _cameraFactory.GetMainCameraAsync(),
                serializableComponents.CharacterSerializableComponents.CameraControllerSerializableComponents,
                _inputService);

            var cursorController = _inputService is IShouldSwitchCursorProvider shouldSwitchCursorProvider
                ? new CursorController(shouldSwitchCursorProvider, sharedSerializableComponents.MonoBehaviourObserver)
                : null;

            var ownerPlayerMovementController = new OwnerPlayerMovementController(
                (await _cameraFactory.GetMainCameraAsync()).transform,
                serializableComponents.CharacterSerializableComponents.MovementSerializableComponents, _inputService,
                sharedSerializableComponents.CharacterSerializableComponents.CharacterController);

            var heroAnimatorController = new HeroAnimatorController(
                sharedSerializableComponents.CharacterSerializableComponents.Animator,
                sharedSerializableComponents.CharacterSerializableComponents.NetworkAnimator);

            var armsAnimatorController = new ArmsAnimatorController(
                serializableComponents.CharacterSerializableComponents.ArmsAnimatorSerializableComponents);

            new OwnerPlayerMovementAnimatorController(ownerPlayerMovementController,
                sharedSerializableComponents.CharacterSerializableComponents.MonoBehaviourObserver,
                cameraController,
                heroAnimatorController, armsAnimatorController,
                sharedSerializableComponents.CharacterSerializableComponents.CharacterController);

            var animatorStateObserver = sharedSerializableComponents.AnimatorSerializableComponents.Animator
                .GetComponent<AnimatorStateObserver>();

            new OwnerPlayerAudioController(ownerPlayerMovementController,
                serializableComponents.CharacterSerializableComponents.MovementSerializableComponents,
                serializableComponents.CharacterSerializableComponents.WalkingAudioSource,
                serializableComponents.CharacterSerializableComponents.JumpAudioSource,
                _heroAudioClipsProviderService, animatorStateObserver,
                sharedSerializableComponents.AnimatorSerializableComponents.Animator);

            cameraController.ChangedCameraMode += isFirstCamera =>
            {
                serializableComponents.ClientSerializableComponents.ChatTextSerializableComponents.TextRootParent
                    .gameObject.SetActive(
                        !isFirstCamera && clientComponents.ChatTextController.IsTextRootParentActive);

                serializableComponents.ClientSerializableComponents.NicknameText.gameObject.SetActive(!isFirstCamera);

                CollectionTools.ToList<ItemCategory>()
                    .Where(category => category is not ItemCategory.All and not ItemCategory.Emotions)
                    .ForEach(category =>
                        clientComponents.ColorableClothesViewController.TrySettingItemCategoryInstancesActiveState(
                            category, !isFirstCamera));
            };

            cameraController.ChangedCameraMode += SetBodyAndArmsEnabledMode;

            void SetBodyAndArmsEnabledMode(bool isFirstCamera)
            {
                clientComponents.SkinColorChangerController.SetBodyEnabledMode(isFirstCamera);
                clientComponents.SkinColorChangerController.SetArmsEnabledMode(isFirstCamera);
            }

            new PlayerAnimatorController(_eventBus,
                sharedSerializableComponents.transform,
                sharedSerializableComponents.AnimatorSerializableComponents,
                animatorStateObserver,
                sharedSerializableComponents.CharacterSerializableComponents.CharacterController);

            var ownerPlayerCharacterComponents = new OwnerPlayerCharacterComponents(
                serializableComponents.CharacterSerializableComponents, ownerPlayerMovementController,
                clientComponents.CharacterComponents, cursorController, cameraController);

            OwnerPlayerComponents =
                new OwnerPlayerComponents(ownerPlayerCharacterComponents, serializableComponents, clientComponents);

            OwnerPlayerInitialized?.Invoke(OwnerPlayerComponents);

            return OwnerPlayerComponents;
        }

        public void DisposePlayers()
        {
            OwnerPlayerComponents = null;
            _clientsComponents.Clear();
        }

        public async UniTask<ClientPlayerComponents> GetInitializedClientPlayerComponents(
            NetworkConnection networkConnection,
            SharedPlayerSerializableComponents sharedSerializableComponents) =>
            await GetInitializedClientPlayerComponents(networkConnection,
                await GetClientPlayerComponentsAsync(sharedSerializableComponents),
                sharedSerializableComponents);

        private async UniTask<ClientPlayerSerializableComponents> GetClientPlayerComponentsAsync(
            SharedPlayerSerializableComponents sharedPlayerCharacterSerializableComponents)
        {
            var serializableComponentsPrefab =
                await _container.GetOrLoadAndRegisterObjectAsync<ClientPlayerSerializableComponents>(PlayerViewAssetId,
                    _assetLoader,
                    shouldCreate: false);

            return UnityEngine.Object.Instantiate(serializableComponentsPrefab,
                sharedPlayerCharacterSerializableComponents.transform);
        }

        public async UniTask<ClientPlayerComponents> GetInitializedClientPlayerComponents(
            NetworkConnection networkConnection, ClientPlayerSerializableComponents serializableComponents,
            SharedPlayerSerializableComponents sharedSerializableComponents)
        {
            var animator = sharedSerializableComponents.CharacterSerializableComponents.Animator;

            var particleEventController = animator.gameObject.AddComponent<ParticleEventController>();
            var animatorStateObserver =
                sharedSerializableComponents.CharacterSerializableComponents.Animator.gameObject
                    .AddComponent<AnimatorStateObserver>();

            serializableComponents.CharacterSerializableComponents.AnimatorChildrenParent
                .ReappointTransformsAndRebindAnimator(animator);

            var chatTextController = await _chatFactory.GetChatTextControllerAsync(
                serializableComponents.ChatTextSerializableComponents,
                await _cameraFactory.GetMainCameraAsync());

            var skinColorChangerController = new SkinColorChangerController(
                serializableComponents.SkinColorChangerSerializableComponents,
                _clientsDataProviderService.Data[networkConnection].CustomizationData.AppearanceData);

            var playerColorableClothesViewController = new ColorableClothesViewController(
                _clientsDataProviderService.Data[networkConnection].CustomizationData.EquippedItemsData,
                await _itemsViewConfigsProviderService.GetItemsViewConfigAsync(),
                serializableComponents.ColorableClothesViewSerializableComponents,
                _clothesFactory);

            _clientsComponents.AddOrAppoint(networkConnection, new ClientPlayerComponents(chatTextController,
                skinColorChangerController,
                playerColorableClothesViewController,
                new PlayerEmotionsComponents(sharedSerializableComponents.EmotionsSerializableComponents,
                    await _emotionsViewConfigsProviderService.GetEmotionsViewConfigAsync(),
                    sharedSerializableComponents.CharacterSerializableComponents.NetworkAnimator, animatorStateObserver,
                    particleEventController),
                new ClientPlayerCharacterComponents(serializableComponents.CharacterSerializableComponents),
                serializableComponents));

            _clientsCustomizationDataProviderService.ChangedClientCustomizationData +=
                (clientWithDataNetworkConnection, data)
                    =>
                {
                    if (clientWithDataNetworkConnection != networkConnection) return;

                    skinColorChangerController.SetDefaultClothesEmissionColorAndSkinColor(
                        data.AppearanceData.SkinColor, data.AppearanceData.DefaultClothesEmissionColor);
                    playerColorableClothesViewController.SetClothesInstancesByData(data.EquippedItemsData);
                };

            serializableComponents.NicknameLooker.Construct(
                await _cameraFactory.GetMainCameraAsync());

            ClientPlayerInitialized?.Invoke(networkConnection, _clientsComponents[networkConnection]);

            return _clientsComponents[networkConnection];
        }
    }
}