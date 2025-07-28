using System;
using Cysharp.Threading.Tasks;
using FishNet.Managing.Client;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public class InitializationGameState : IGameState
    {
        private readonly ClientManager _clientManager;

        public InitializationGameState(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public async UniTask EnterAsync()
        {
            _clientManager.StartConnection((await ClientFilesLoadingTools.LoadClientSettingsAsync()).Address, 7777);
        }

        public UniTask ExitAsync() => UniTask.CompletedTask;
    }
}