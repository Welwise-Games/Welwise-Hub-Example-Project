using System;
using System.Collections.Generic;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services
{
    public class BotsNicknamesProviderService
    {
        public IReadOnlyDictionary<int, string> Nicknames => _nicknames;
        public event Action<int, string> ChangedBotNickname;
        
        private readonly Dictionary<int, string> _nicknames = new Dictionary<int, string>();

        public void AddBotNickname(int botObjectId, string newNickname)
            => _nicknames.TryAdd(botObjectId, newNickname);
        
        public void TrySettingBotNickname(int botObjectId, string newNickname)
        {
            if (!_nicknames.ContainsKey(botObjectId))
                return;
            
            _nicknames[botObjectId] = newNickname;
            ChangedBotNickname?.Invoke(botObjectId, newNickname);
        }

        public void RemoveBotNickname(int botObjectId) => _nicknames.Remove(botObjectId);
        public void Dispose() => _nicknames.Clear();
    }
}