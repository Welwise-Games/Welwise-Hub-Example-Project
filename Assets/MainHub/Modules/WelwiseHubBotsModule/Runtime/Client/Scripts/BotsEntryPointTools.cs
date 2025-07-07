using FishNet.Managing.Client;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseHubBotsModule.Runtime.Client.Scripts
{
    public static class BotsEntryPointTools
    {
        public static void Initialize(CameraFactory cameraFactory, ClientManager clientManager,
            ClientsConnectionTrackingServiceForClient connectionTrackingService,
            EmotionsConfigsProviderService emotionsConfigsProviderService,
            EmotionsViewConfigsProviderService emotionsViewConfigsProviderService,
            EmotionsViewFactory emotionsViewFactory, 
            ItemsViewConfigsProviderService itemsViewConfigsProviderService,
            ClothesFactory clothesFactory, IAssetLoader assetLoader)
        {
            var botsNicknamesProviderService = new BotsNicknamesProviderService();
            var botsCustomizationDataProviderService = new BotsCustomizationDataProviderService();

            var botsFactory = new BotsFactory(cameraFactory, emotionsViewConfigsProviderService, 
                botsNicknamesProviderService, botsCustomizationDataProviderService,
                itemsViewConfigsProviderService, clothesFactory, assetLoader);

            botsFactory.InitializedBot += (botGameObject, botObjectId) =>
                botGameObject.GetOrAddComponent<DestroyObserver>().Destroyed += () =>
                {
                    botsNicknamesProviderService.RemoveBotNickname(botObjectId);
                    botsCustomizationDataProviderService.RemoveBotCustomizationData(botObjectId);
                };

            connectionTrackingService.OwnerDisconnected += botsFactory.Dispose;
            connectionTrackingService.OwnerDisconnected += botsNicknamesProviderService.Dispose;
            connectionTrackingService.OwnerDisconnected += botsCustomizationDataProviderService.Dispose;

            var synchronizer =
                new BotsClientSynchronizer(botsFactory, emotionsConfigsProviderService, emotionsViewFactory,
                    botsNicknamesProviderService,
                    botsCustomizationDataProviderService);

            clientManager.RegisterBroadcast<PlayBotEmotionBroadcast>(synchronizer.PlayBotEmotionAsync);
            clientManager.RegisterBroadcast<InitializationBotBroadcast>(synchronizer.InitializeBot);
            clientManager.RegisterBroadcast<BotChangedNicknameBroadcast>(synchronizer.ChangeBotNickname);
            clientManager.RegisterBroadcast<BotChangedCustomizationDataBroadcast>(synchronizer
                .ChangeBotCustomizationData);
        }
    }
}