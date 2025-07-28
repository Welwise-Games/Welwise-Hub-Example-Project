using FishNet.Managing.Server;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseSharedModule.Runtime.Server.Scripts;

namespace WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network
{
    public static class EmotionsEntryPointTools
    {
        public static void Initialize(ServerManager serverManager,
            IVisibleClientsProviderService visibleClientsProviderService,
            EmotionsConfigProviderService emotionsConfigProviderService, EmotionsAnimationsConfig emotionsAnimationsConfig,
            out EmotionsEntryPointData emotionsEntryPointData)
        {
            var clientsSelectedEmotionsDataProviderService = new ClientsSelectedEmotionsDataProviderService(emotionsAnimationsConfig);
            
            new ServerEmotionsPlayingSynchronizerService(clientsSelectedEmotionsDataProviderService,
                visibleClientsProviderService,
                serverManager, emotionsConfigProviderService);

            emotionsEntryPointData = new EmotionsEntryPointData(clientsSelectedEmotionsDataProviderService);
        }
    }
}