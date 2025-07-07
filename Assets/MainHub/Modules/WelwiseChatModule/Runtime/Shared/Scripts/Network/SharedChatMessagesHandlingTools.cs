using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChatModule.Runtime.Shared.Scripts.Network
{
    public static class SharedChatMessagesHandlingTools
    {
        public static string ForbiddenWordsFileName => "forbidden_words.txt";

        public static string GetReplacingForbiddenWords(string content, string forbiddenWordsFileText)
        {
            var forbiddenWordsContent = forbiddenWordsFileText?.Split("\n");
            
            var signForReplacing = forbiddenWordsContent?.FirstOrDefault();

            if (signForReplacing == null)
            {
                Debug.Log("Sign for replacing is empty. Insert it in first line!");
                return content;
            }

            var forbiddenWords = forbiddenWordsContent[1..].Where(word => !word.IsNullOrEmptyOrWhiteSpace()).Select(
                word => word.Replace("\r", "")).ToArray();

            if (forbiddenWords.Length == 0)
            {
                Debug.Log("Forbidden words are empty. Insert after first line (each word is on a separate line)");
                return content;
            }

            var pattern = string.Join("|", forbiddenWords);

            content = Regex.Replace(content, pattern, word => new string('*', word.Length), RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\s+", " ").Trim();

            return content;
        }
    }
}