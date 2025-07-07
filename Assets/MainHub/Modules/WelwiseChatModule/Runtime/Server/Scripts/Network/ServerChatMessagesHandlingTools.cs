using System.IO;
using UnityEngine;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace Modules.WelwiseChatModule.Runtime.Server.Scripts.Network
{
    public static class ServerChatMessagesHandlingTools
    {
        private static string ForbiddenWordsFilePath =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", SharedChatMessagesHandlingTools.ForbiddenWordsFileName));
        
        public static string GetProcessedMessageContentForServer(this string content)
        {
            var forbiddenWordsFilePath = ForbiddenWordsFilePath;

            if (File.Exists(forbiddenWordsFilePath) || (content = content.GetWithoutTags()).IsNullOrEmptyOrWhiteSpace())
                return SharedChatMessagesHandlingTools.GetReplacingForbiddenWords(content, File.ReadAllText(forbiddenWordsFilePath));
            
            Debug.Log($"Forbidden words path doesn't exist. Required path: {forbiddenWordsFilePath}");
            return content;
        }
    }
}