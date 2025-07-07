using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FishNet.Connection;

namespace WelwiseChangingNicknameModule.Runtime.Server.Scripts.Services
{
    public interface ICanSetNicknameConditionProvider
    {
        UniTask<bool> CanSetAsync(NetworkConnection networkConnection, string newNickname);
    }
}