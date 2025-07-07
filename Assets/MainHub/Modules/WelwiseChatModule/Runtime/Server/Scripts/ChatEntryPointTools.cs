using FishNet.Managing.Server;
using Modules.WelwiseChatModule.Runtime.Server.Scripts.Network;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;

namespace Modules.WelwiseChatModule.Runtime.Server.Scripts
{
    public static class ChatEntryPointTools
    {
        public static void Initialize(IServerChatsDataProvider serverChatsDataProvider,
            IClientsNetworkConnectionsProviderService clientsNetworkConnectionsProviderService,
            IClientsNicknamesProviderService clientsCustomizationDataProviderService, ServerManager serverManager)
        {
            new ServerChatSynchronizer(clientsNetworkConnectionsProviderService,
                clientsCustomizationDataProviderService, serverChatsDataProvider, serverManager);
        }
    }
}