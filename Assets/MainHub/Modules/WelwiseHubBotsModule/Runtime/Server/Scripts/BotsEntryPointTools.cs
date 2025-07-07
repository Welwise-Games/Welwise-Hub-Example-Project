using FishNet;
using FishNet.Managing.Server;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public static class BotsEntryPointTools
    {
        public static void Initialize(
            SetPlayerAnimationPlaceModelsProviderService setPlayerAnimationPlaceModelsProviderService,
            ServerSceneManagementService serverSceneManagementService,
            IRoomsProviderService roomsProviderService, ServerManager serverManager, string roomSceneName,
            EmotionsConfigsProviderService emotionsConfigsProviderService, out BotsEntryPointData botsEntryPointData,
            ServerSetPlayersAnimationsPlacesSynchronizer serverSetPlayersAnimationsPlacesSynchronizer,
            ClientsConfigsProviderService clientsConfigsProviderService,
            ItemsConfigsProviderService itemsConfigsProviderService, IAssetLoader assetLoader)
        {
            var botsConfigsProviderService = new BotsConfigsProviderService(assetLoader);

            var botsNicknamesProviderService = new BotsNicknamesProviderService();
            var botsCustomizationDataProviderService = new BotsCustomizationDataProviderService();

            var botsFactory = new BotsFactory(botsConfigsProviderService, setPlayerAnimationPlaceModelsProviderService,
                roomsProviderService, emotionsConfigsProviderService, serverSetPlayersAnimationsPlacesSynchronizer,
                botsNicknamesProviderService, botsCustomizationDataProviderService, clientsConfigsProviderService, itemsConfigsProviderService);

            botsEntryPointData = new BotsEntryPointData(botsFactory, botsConfigsProviderService);

            new BotsSubscribingMediator(botsNicknamesProviderService, serverManager, roomSceneName, botsFactory,
                serverSceneManagementService, roomsProviderService, botsCustomizationDataProviderService,
                clientsConfigsProviderService, botsConfigsProviderService, itemsConfigsProviderService);
        }
    }
}