using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChatModule.Runtime.Client.Scripts.Network
{
    public class ChatsDataProviderService
    {
        public IReadOnlyDictionary<ChatZone, IReadOnlyCollection<ChatMessageData>> DataByChatZone =>
            _dataByChatZone.ToDictionary(data => data.Key,
                data => data.Value as IReadOnlyCollection<ChatMessageData>);

        public event Action<NetworkConnection, ChatZone, ChatMessageData> AddedMessageData;

        public event Action<NetworkConnection, ChatZone, ChatMessageData, string> AddedOwnerMessageData;

        public event Action AddedInitializationMessagesData;

        private readonly Dictionary<ChatZone, HashSet<ChatMessageData>> _dataByChatZone =
            new Dictionary<ChatZone, HashSet<ChatMessageData>>();

        private readonly string _gotForbiddenWordsFileText;
        
        public ChatsDataProviderService(string forbiddenWordsText) => _gotForbiddenWordsFileText = forbiddenWordsText;

        public void TryAddingInitializationMessagesData(Dictionary<ChatZone, List<ChatMessageData>> messagesData)
        {
            var doesAddedAnyMessage = false;

            foreach (var pair in messagesData)
            {
                foreach (var messageData in pair.Value)
                {
                    TryProcessingAndAddingMessageData(pair.Key, messageData, out var successfully, shouldProcessContent: false);
                    doesAddedAnyMessage = doesAddedAnyMessage || successfully;
                }
            }

            if (doesAddedAnyMessage)
                AddedInitializationMessagesData?.Invoke();
        }

        public void ClearMessagesData() => _dataByChatZone.Clear();

        public void TryProcessingAndAddingMessageData(ChatZone chatZone, ChatMessageData chatMessageData,
            out bool successfully, NetworkConnection senderNetworkConnection = null, bool shouldProcessContent = true)
        {
            successfully = false;

            var inputMessageContent = chatMessageData.Content;
            
            var processedContent = shouldProcessContent
                ? chatMessageData.Content.GetProcessedMessageContentForClient(_gotForbiddenWordsFileText)
                : chatMessageData.Content;
            
            if (chatMessageData.AuthorNickname.IsNullOrEmptyOrWhiteSpace() ||
                processedContent.IsNullOrEmptyOrWhiteSpace())
                return;

            chatMessageData.Content = processedContent;

            if (!_dataByChatZone.ContainsKey(chatZone))
                _dataByChatZone.Add(chatZone, new HashSet<ChatMessageData>());

            if (!_dataByChatZone[chatZone].Add(chatMessageData)) return;

            successfully = true;

            if (senderNetworkConnection.IsOwners())
                AddedOwnerMessageData?.Invoke(senderNetworkConnection, chatZone, chatMessageData, inputMessageContent);

            if (senderNetworkConnection != null)
                AddedMessageData?.Invoke(senderNetworkConnection, chatZone, chatMessageData);
        }
    }
}