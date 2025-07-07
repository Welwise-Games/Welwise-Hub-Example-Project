using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using Modules.WelwiseChatModule.Runtime.Server.Scripts;
using Modules.WelwiseChatModule.Runtime.Server.Scripts.Network;
using UnityEngine;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.ChatSystem
{
    public class ServerChatsDataProvider : IServerChatsDataProvider
    {
        private readonly Dictionary<Hub, Dictionary<ChatZone, HashSet<ChatMessageData>>> _messagesByHub =
            new Dictionary<Hub, Dictionary<ChatZone, HashSet<ChatMessageData>>>();

        private readonly HubsProviderService _hubsProviderService;

        public ServerChatsDataProvider(HubsProviderService hubsProviderService)
        {
            _hubsProviderService = hubsProviderService;
        }

        public IReadOnlyDictionary<ChatZone, IReadOnlyCollection<ChatMessageData>> GetChatsMessagesData(
            NetworkConnection networkConnection) =>
            !_hubsProviderService.HubByPlayerNetworkConnection.TryGetValue(networkConnection, out var hub)
                ? null
                : _messagesByHub.GetValueOrDefault(hub)?.ToDictionary(pair => pair.Key,
                    pair => pair.Value as IReadOnlyCollection<ChatMessageData>);

        public void TryProcessingAndAddingChatMessageData(NetworkConnection networkConnection, ChatZone chatZone,
            ChatMessageData chatMessageData, out bool successfully, out string processedMessageContent)
        {
            successfully = false;

            processedMessageContent = chatMessageData.Content.GetProcessedMessageContentForServer();

            if (!_hubsProviderService.HubByPlayerNetworkConnection.TryGetValue(networkConnection, out var hub) || processedMessageContent.IsNullOrEmptyOrWhiteSpace())
                return;

            _messagesByHub.TryAdd(hub, new Dictionary<ChatZone, HashSet<ChatMessageData>>());
            _messagesByHub[hub].TryAdd(chatZone, new HashSet<ChatMessageData>());
            
            successfully = _messagesByHub[hub][chatZone].Add(chatMessageData);

            Debug.Log($"Hub ID: {hub.Id}. Chat Zone: {chatZone}. Author: {chatMessageData.AuthorNickname}. Message: {chatMessageData.Content}");
        }

        public void TryRemovingHubMessagesData(Hub hub) => _messagesByHub.Remove(hub);
    }
}