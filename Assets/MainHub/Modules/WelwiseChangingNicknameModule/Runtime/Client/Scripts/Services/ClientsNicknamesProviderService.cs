using System;
using System.Collections.Generic;
using FishNet.Connection;
using UnityEngine;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;

namespace WelwiseChangingNicknameModule.Runtime.Client.Scripts.Services
{
    public class ClientsNicknamesProviderService
    {
        public IReadOnlyDictionary<NetworkConnection, string> Nicknames =>
            _sharedClientsNicknamesProviderService.Nicknames;

        public event Action<NetworkConnection, string> ChangedClientNickname
        {
            add => _sharedClientsNicknamesProviderService.ChangedClientNickname += value;
            remove => _sharedClientsNicknamesProviderService.ChangedClientNickname -= value;
        }

        private readonly SharedClientsNicknamesProviderService _sharedClientsNicknamesProviderService;
        private readonly IClientsNicknamesDataProvider _clientsNicknamesDataProvider;

        public ClientsNicknamesProviderService(SharedClientsNicknamesProviderService sharedClientsNicknamesProviderService, IClientsNicknamesDataProvider clientsNicknamesDataProvider)
        {
            _sharedClientsNicknamesProviderService = sharedClientsNicknamesProviderService;
            _clientsNicknamesDataProvider = clientsNicknamesDataProvider;
        }

        public bool CanSetClientNickname(NetworkConnection networkConnection, string newNickname)
            => _sharedClientsNicknamesProviderService.CanSetClientNickname(
                _clientsNicknamesDataProvider.ClientsNicknameDataProviders.GetValueOrDefault(networkConnection)?.Nickname, newNickname);

        public void TrySettingClientNickname(NetworkConnection networkConnection, string newNickname) =>
            _sharedClientsNicknamesProviderService.TrySettingClientNickname(
                networkConnection, newNickname, false);
    }
}