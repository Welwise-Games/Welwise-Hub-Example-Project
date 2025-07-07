using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Transporting;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseChatModule.Runtime.Shared.Scripts.Network.Dependencies;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;

namespace WelwiseChatModule.Runtime.Client.Scripts.Network
{
    public class OwnerChatSynchronizer
    {
        private readonly ChatsDataProviderService _chatsDataProviderService;
        private readonly ClientManager _clientManager;

        public OwnerChatSynchronizer(ChatsDataProviderService chatsDataProviderService, ClientManager clientManager,
            ClientsConnectionTrackingServiceForClient clientsConnectionTrackingServiceForClient)
        {
            _chatsDataProviderService = chatsDataProviderService;
            _clientManager = clientManager;
            Subscribe(clientsConnectionTrackingServiceForClient);
        }

        private void SendMessageBroadcast(NetworkConnection networkConnection, ChatZone chatZone, ChatMessageData messageData, string inputMessageContent)
            => _clientManager.Broadcast(new SendingMessageDependencies(inputMessageContent, chatZone));

        private void HandleInitializationChats(InitializationChatsDependencies dependencies, Channel channel) =>
            _chatsDataProviderService.TryAddingInitializationMessagesData(dependencies.Chats);

        private void HandleGotMessage(GettingMessageDependencies dependencies, Channel channel) 
            => HandleGotMessage(dependencies.ChatZone, dependencies.MessageData, dependencies.SenderNetworkConnection);

        private void HandleGotMessage(ChatZone chatZone, ChatMessageData chatMessageData, NetworkConnection senderNetworkConnection)
            => _chatsDataProviderService.TryProcessingAndAddingMessageData(chatZone, chatMessageData, out var successfully, senderNetworkConnection, false);

        private void Subscribe(ClientsConnectionTrackingServiceForClient clientsConnectionTrackingServiceForClient)
        {
            _clientManager.RegisterBroadcast<GettingMessageDependencies>(HandleGotMessage);
            _clientManager.RegisterBroadcast<InitializationChatsDependencies>(HandleInitializationChats);
            clientsConnectionTrackingServiceForClient.OwnerDisconnected += _chatsDataProviderService.ClearMessagesData;
            _chatsDataProviderService.AddedOwnerMessageData += SendMessageBroadcast;
        }
    }
}