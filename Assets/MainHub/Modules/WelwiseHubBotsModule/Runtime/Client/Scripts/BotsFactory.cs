using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Client.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseNicknameSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseHubBotsModule.Runtime.Client.Scripts
{
    public class BotsFactory
    {
        public event Action<GameObject, int> InitializedBot;

        public IReadOnlyDictionary<GameObject, EmotionsAnimatorController> BotsEmotionsAnimatorControllers =>
            _botsEmotionsAnimatorControllers;

        private readonly Dictionary<GameObject, EmotionsAnimatorController> _botsEmotionsAnimatorControllers =
            new Dictionary<GameObject, EmotionsAnimatorController>();


        private readonly CameraFactory _cameraFactory;
        private readonly EmotionsViewConfigsProviderService _emotionsViewConfigsProviderService;
        private readonly BotsNicknamesProviderService _botsNicknamesProviderService;
        private readonly BotsCustomizationDataProviderService _botsCustomizationDataProviderService;
        private readonly ItemsViewConfigsProviderService _itemsViewConfigsProviderService;
        private readonly ClothesFactory _clothesFactory;
        private readonly IAssetLoader _assetLoader;
        
        private readonly Container _container = new Container();

        private const string BotViewAssetId = 
#if ADDRESSABLES
        "BotView";
#else
        "WelwiseHubBotsModule/Runtime/Server/Loadable/BotView";
#endif

        public BotsFactory(CameraFactory cameraFactory, EmotionsViewConfigsProviderService emotionsViewConfigsProviderService,
            BotsNicknamesProviderService botsNicknamesProviderService,
            BotsCustomizationDataProviderService botsCustomizationDataProviderService,
            ItemsViewConfigsProviderService itemsViewConfigsProviderService, ClothesFactory clothesFactory, IAssetLoader assetLoader)
        {
            _cameraFactory = cameraFactory;
            _emotionsViewConfigsProviderService = emotionsViewConfigsProviderService;
            _botsNicknamesProviderService = botsNicknamesProviderService;
            _botsCustomizationDataProviderService = botsCustomizationDataProviderService;
            _clothesFactory = clothesFactory;
            _assetLoader = assetLoader;
            _itemsViewConfigsProviderService = itemsViewConfigsProviderService;
        }

        public void Dispose() => _botsEmotionsAnimatorControllers.Clear();

        public async void InitializeBotAsync(SharedBotSerializableComponents sharedSerializableComponents)
        {
            var serializableComponents = UnityEngine.Object.Instantiate(
                await _container.GetOrLoadAndRegisterObjectAsync<ClientBotSerializableComponents>(BotViewAssetId, _assetLoader,
                    shouldCreate: false), sharedSerializableComponents.transform);

            var animator = sharedSerializableComponents.Animator;

            var particleEventController = animator.gameObject.AddComponent<ParticleEventController>();
            var animatorStateObserver =
                animator.gameObject.AddComponent<AnimatorStateObserver>();

            serializableComponents.AnimatorChildrenParent.ReappointTransformsAndRebindAnimator(animator);

            serializableComponents.ToCameraLooker.Construct(await _cameraFactory.GetMainCameraAsync());

            _botsEmotionsAnimatorControllers.Add(sharedSerializableComponents.gameObject,
                new EmotionsAnimatorController(
                    sharedSerializableComponents.EmotionsSerializableComponents.Animator,
                    animatorStateObserver,
                    particleEventController,
                    await _emotionsViewConfigsProviderService.GetEmotionsViewConfigAsync()));

            var botObjectId = sharedSerializableComponents.GetComponent<NetworkObject>().ObjectId;

            var nicknameTextController = new PlayerNicknameTextController(
                serializableComponents.NicknameText, _botsNicknamesProviderService.Nicknames[botObjectId]);

            var clothesController = new ColorableClothesViewController(
                _botsCustomizationDataProviderService.BotsCustomizationData[botObjectId].EquippedItemsData,
                await _itemsViewConfigsProviderService.GetItemsViewConfigAsync(),
                serializableComponents.ColorableClothesViewSerializableComponents, _clothesFactory);

            var skinColorChangerController = new SkinColorChangerController(
                serializableComponents.SkinColorChangerSerializableComponents,
                _botsCustomizationDataProviderService.BotsCustomizationData[botObjectId].AppearanceData);

            _botsCustomizationDataProviderService.ChangedBotCustomizationData +=
                TryChangingSkinColorAndClothesInstances;

            sharedSerializableComponents.gameObject.GetOrAddComponent<DestroyObserver>().Destroyed += () =>
                _botsCustomizationDataProviderService.ChangedBotCustomizationData -=
                    TryChangingSkinColorAndClothesInstances;

            void TryChangingSkinColorAndClothesInstances(int objectId, CustomizationData data)
            {
                if (objectId != botObjectId) return;

                skinColorChangerController.SetDefaultClothesEmissionColorAndSkinColor(
                    data.AppearanceData.SkinColor, data.AppearanceData.DefaultClothesEmissionColor);
                clothesController.SetClothesInstancesByData(data.EquippedItemsData);
            }

            NicknameChangingTools.InitializeBot(_botsNicknamesProviderService, nicknameTextController,
                botObjectId);
            
            sharedSerializableComponents.NavMeshAgent.enabled = false;

            InitializedBot?.Invoke(sharedSerializableComponents.gameObject, botObjectId);
        }
    }
}