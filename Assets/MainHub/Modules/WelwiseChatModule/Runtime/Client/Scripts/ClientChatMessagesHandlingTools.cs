using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChatModule.Runtime.Client.Scripts
{
    public static class ClientChatMessagesHandlingTools
    {
        private static string ForbiddenWordsFilePath =>
            Path.Combine(Application.streamingAssetsPath, SharedChatMessagesHandlingTools.ForbiddenWordsFileName);

        public static async UniTask<string> GetForbiddenWordsFileTextForClient()
        {
            string forbiddenWordsFileText = null;
            
            using (var request = UnityWebRequest.Get(ForbiddenWordsFilePath))
            {
                try
                {
                    await request.SendWebRequest();
                    
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Failed to load forbidden words text file: " + request.error);
                    }
                    else
                    {
                        forbiddenWordsFileText = request.downloadHandler.text;
                        Debug.Log($"Loaded forbidden text file: Address = {ForbiddenWordsFilePath}");
                    }   
                }
                catch
                {
                    Debug.LogError("Failed to load forbidden words text file: " + request.error);
                }
            }

            return forbiddenWordsFileText;
        }

        public static string GetProcessedMessageContentForClient(this string content, string forbiddenWordsFileText) =>
            forbiddenWordsFileText == null || (content = content.GetWithoutTags()).IsNullOrEmptyOrWhiteSpace()
                ? content
                : SharedChatMessagesHandlingTools.GetReplacingForbiddenWords(content, forbiddenWordsFileText);
    }
}