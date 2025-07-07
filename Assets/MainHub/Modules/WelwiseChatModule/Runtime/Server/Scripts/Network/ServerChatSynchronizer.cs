using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseChatModule.Runtime.Shared.Scripts.Network.Dependencies;

namespace Modules.WelwiseChatModule.Runtime.Server.Scripts.Network
{
    public class ServerChatSynchronizer
    {
        private readonly IClientsNetworkConnectionsProviderService _clientsNetworkConnectionsProviderService;
        private readonly IServerChatsDataProvider _serverChatsDataProvider;
        private readonly ServerManager _serverManager;
        private readonly IClientsNicknamesProviderService _clientsNicknamesProviderService;

        public ServerChatSynchronizer(
            IClientsNetworkConnectionsProviderService clientsNetworkConnectionsProviderService,
            IClientsNicknamesProviderService clientsNicknamesProviderService,
            IServerChatsDataProvider serverChatsDataProvider, ServerManager serverManager)
        {
            _clientsNetworkConnectionsProviderService = clientsNetworkConnectionsProviderService;
            _clientsNicknamesProviderService = clientsNicknamesProviderService;
            _serverChatsDataProvider = serverChatsDataProvider;
            _serverManager = serverManager;
            Subscribe();
        }

        private void Subscribe() =>
            _serverManager.RegisterBroadcast<SendingMessageDependencies>(HandleSendMessage);

        private void HandleSendMessage(NetworkConnection senderNetworkConnection,
            SendingMessageDependencies sendingMessageDependencies, Channel channel)
        {
            var clientsNetworkConnections =
                _clientsNetworkConnectionsProviderService.GetClientsNetworkConnections(senderNetworkConnection,
                    sendingMessageDependencies.ChatZone);

            if (clientsNetworkConnections == null || !_clientsNicknamesProviderService.Nicknames.TryGetValue(
                                                      senderNetworkConnection, out var clientNickname))
                return;

            _serverChatsDataProvider.TryProcessingAndAddingChatMessageData(senderNetworkConnection,
                sendingMessageDependencies.ChatZone, new ChatMessageData(sendingMessageDependencies.Message,
                    clientNickname), out var successfully, out var processedMessageContent);

            if (!successfully) 
                return;

            var dependencies = new GettingMessageDependencies(sendingMessageDependencies.ChatZone,
                new ChatMessageData(processedMessageContent, clientNickname), senderNetworkConnection);
            
            foreach (var networkConnection in clientsNetworkConnections.Where(connection =>
                connection != senderNetworkConnection)) 
                _serverManager.Broadcast(networkConnection, dependencies);
        }
    }
}