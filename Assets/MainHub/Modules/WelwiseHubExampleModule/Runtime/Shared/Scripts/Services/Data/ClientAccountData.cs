using System;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data
{
    [Serializable]
    public class ClientAccountData : IClientNicknameData
    {
        public string Nickname { get; set; }
        public string Id { get; set; }

        public ClientAccountData(string id, string nickname)
        {
            Id = id;
            Nickname = nickname;
        }

        public ClientAccountData()
        {
            
        }
    }
}