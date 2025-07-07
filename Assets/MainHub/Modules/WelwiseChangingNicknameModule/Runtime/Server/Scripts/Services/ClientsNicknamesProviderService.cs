using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using UnityEngine;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;

namespace WelwiseChangingNicknameModule.Runtime.Server.Scripts.Services
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
        private readonly ICanSetNicknameConditionProvider _canSetNicknameConditionProvider;


        public ClientsNicknamesProviderService(SharedClientsNicknamesProviderService sharedClientsNicknamesProviderService,
            ICanSetNicknameConditionProvider canSetNicknameConditionProvider)
        {
            _sharedClientsNicknamesProviderService = sharedClientsNicknamesProviderService;
            _canSetNicknameConditionProvider = canSetNicknameConditionProvider;
        }

        public async UniTask TrySettingClientNicknameAsync(NetworkConnection networkConnection, string newNickname)
        {
            if (!await _canSetNicknameConditionProvider.CanSetAsync(networkConnection, newNickname))
                return;

            _sharedClientsNicknamesProviderService.TrySettingClientNickname(networkConnection, newNickname);
        }
    }
}