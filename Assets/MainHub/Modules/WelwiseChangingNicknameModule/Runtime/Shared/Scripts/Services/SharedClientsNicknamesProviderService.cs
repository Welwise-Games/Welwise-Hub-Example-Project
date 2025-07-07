using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using UnityEngine;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services
{
    public class SharedClientsNicknamesProviderService
    {
        public IReadOnlyDictionary<NetworkConnection, string> Nicknames =>
            _clientsDataProviderService.ClientsNicknameDataProviders.ToDictionary(pair => pair.Key,
                pair => pair.Value.Nickname);

        public event Action<NetworkConnection, string> ChangedClientNickname;


        private readonly IClientsNicknamesDataProvider _clientsDataProviderService;
        private readonly SharedClientsNicknamesConfig _sharedClientsNicknamesConfig;

        public SharedClientsNicknamesProviderService(SharedClientsNicknamesConfig sharedClientsNicknamesConfig, IClientsNicknamesDataProvider clientsDataProviderService)
        {
            _sharedClientsNicknamesConfig = sharedClientsNicknamesConfig;
            _clientsDataProviderService = clientsDataProviderService;
        }

        public bool CanSetClientNickname(string oldNickname, string newNickname,
            bool shouldCheckIsNicknameCorrect = true) => oldNickname != null && (!shouldCheckIsNicknameCorrect ||
            DataValidationTools.IsValidNickname(
                oldNickname, newNickname,
                _sharedClientsNicknamesConfig));

        public void TrySettingClientNickname(NetworkConnection networkConnection, string newNickname,
            bool shouldCheckIsNicknameCorrect = true)
        {
            var dataProvider =
                _clientsDataProviderService.ClientsNicknameDataProviders.GetValueOrDefault(networkConnection);
            
            if (!CanSetClientNickname(dataProvider.Nickname, newNickname, shouldCheckIsNicknameCorrect))
                return;

            dataProvider.Nickname = newNickname;

            ChangedClientNickname?.Invoke(networkConnection, newNickname);
        }
    }
}