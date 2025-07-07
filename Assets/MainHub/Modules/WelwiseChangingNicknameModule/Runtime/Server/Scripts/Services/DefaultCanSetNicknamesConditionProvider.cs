using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChangingNicknameModule.Runtime.Server.Scripts.Services
{
    public class DefaultCanSetNicknamesConditionProvider : ICanSetNicknameConditionProvider
    {
        private readonly HashSet<string> _forbiddenNicknames;

        public DefaultCanSetNicknamesConditionProvider() =>
            _forbiddenNicknames = ReadingFileTools.GetWordsFromFile("forbidden_nicknames.txt");

        public async UniTask<bool> CanSetAsync(NetworkConnection networkConnection, string newNickname) =>
            !DoesContainInForbiddenNicknames(newNickname);

        private bool DoesContainInForbiddenNicknames(string nickname) => _forbiddenNicknames.Count != 0 && Regex.IsMatch(nickname,
             string.Join("|", _forbiddenNicknames), RegexOptions.IgnoreCase);
    }
}