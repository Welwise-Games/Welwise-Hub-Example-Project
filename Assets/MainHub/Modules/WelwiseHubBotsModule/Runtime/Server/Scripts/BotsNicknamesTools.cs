using System.IO;
using System.Linq;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public static class BotsNicknamesTools
    {
        private const string GhostNickname = "Ghost";

        public static string GetRandomNickname(string exceptNickname = null)
        {
            var nicknames = ReadingFileTools.GetWordsFromFile("bots_nicknames.txt");

            if (nicknames == null || nicknames.Count == 0)
                return GhostNickname;

            return exceptNickname != null
                ? nicknames.Where(nickname => nickname != exceptNickname).GetRandomOrDefault()
                : nicknames.GetRandomOrDefault();
        }
    }
}