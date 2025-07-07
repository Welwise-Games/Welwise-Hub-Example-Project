using Cysharp.Threading.Tasks;
using FishNet.Managing.Client;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseChatModule.Runtime.Client.Scripts.Network
{
    public static class ChatEntryPointTools
    {
        public static async UniTask InitializeAsync(ClientManager clientManager,
            ClientsConnectionTrackingServiceForClient clientsConnectionTrackingServiceForClient,
            IClientsNicknamesProviderService clientsNicknamesProviderService,
            DataContainer<ChatEntryPointData> dataContainer, IAssetLoader assetLoader)
        {
            var ownerChatsDataProviderService =
                new ChatsDataProviderService(await ClientChatMessagesHandlingTools
                    .GetForbiddenWordsFileTextForClient());
            
            var chatFactory = new ChatFactory(ownerChatsDataProviderService, clientsNicknamesProviderService, assetLoader);
            
            new OwnerChatSynchronizer(ownerChatsDataProviderService, clientManager, clientsConnectionTrackingServiceForClient);

            dataContainer.Data = new ChatEntryPointData(chatFactory, ownerChatsDataProviderService);
        }
    }
}