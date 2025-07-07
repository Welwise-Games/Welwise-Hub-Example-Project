using System.Collections.Generic;
using FishNet.Connection;

namespace WelwiseChatModule.Runtime.Shared.Scripts.Network
{
    public interface IClientsNicknamesProviderService
    {
        IReadOnlyDictionary<NetworkConnection, string> Nicknames { get; }
    }
}