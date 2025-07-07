using System;
using System.Collections.Generic;
using FishNet.Connection;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services
{
    public interface IClientsNicknamesDataProvider
    {
        IReadOnlyDictionary<NetworkConnection, IClientNicknameData> ClientsNicknameDataProviders { get; }
        event Action<NetworkConnection, IClientNicknameData> AddedNicknameData;
    }
}