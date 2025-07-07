using FishNet.Managing.Server;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Server.Scripts;

namespace WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network
{
    public static class EmotionsEntryPointTools
    {
        public static void Initialize(ServerManager serverManager,
            IVisibleClientsProviderService visibleClientsProviderService,
            EmotionsConfigsProviderService emotionsConfigsProviderService,
            out EmotionsEntryPointData emotionsEntryPointData)
        {
            var clientsSelectedEmotionsDataProviderService = new ClientsSelectedEmotionsDataProviderService();
            
            new ServerEmotionsPlayingSynchronizerService(clientsSelectedEmotionsDataProviderService,
                visibleClientsProviderService,
                serverManager, emotionsConfigsProviderService);

            emotionsEntryPointData = new EmotionsEntryPointData(clientsSelectedEmotionsDataProviderService);
        }
    }
}