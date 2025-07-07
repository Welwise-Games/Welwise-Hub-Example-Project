using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing.Server;
using UnityEngine;
using UnityEngine.SceneManagement;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Object = UnityEngine.Object;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.PlayerSystem
{
    public class PlayersFactory
    {
        public IReadOnlyDictionary<NetworkConnection, SharedPlayerSerializableComponents> CreatedPlayers =>
            _createdPlayers;

        public event Action<SharedPlayerSerializableComponents, NetworkConnection> Created;

        private readonly Dictionary<NetworkConnection, SharedPlayerSerializableComponents> _createdPlayers =
            new Dictionary<NetworkConnection, SharedPlayerSerializableComponents>();

        private readonly PlayersConfigsProviderService _playersConfigsProviderService;
        private readonly ServerManager _serverManager;
        
        private readonly Container _container = new Container();
        private readonly IAssetLoader _assetLoader;

        public const string PlayerAssetId = 
#if ADDRESSABLES
        "Player";
#else
        "WelwiseHubExampleModule/Runtime/Shared/Loadable/Player";
#endif

        public PlayersFactory(ServerManager serverManager, PlayersConfigsProviderService playersConfigsProviderService, IAssetLoader assetLoader)
        {
            _serverManager = serverManager;
            _playersConfigsProviderService = playersConfigsProviderService;
            _assetLoader = assetLoader;
        }

        public async UniTask<SharedPlayerSerializableComponents> GetInitializedOwnerPlayerAsync(
            NetworkConnection networkConnection, Scene targetScene)
        {
            var prefab =
                await _container.GetOrLoadAndRegisterObjectAsync<SharedPlayerSerializableComponents>(
                    PlayerAssetId, _assetLoader, shouldCreate: false);

            var playersConfig = await _playersConfigsProviderService.GetPlayersConfigAsync();
            
            var instance = Object.Instantiate(prefab, playersConfig.SpawnPosition, Quaternion.identity);

            SceneManager.MoveGameObjectToScene(instance.gameObject, targetScene);
            _serverManager.Spawn(instance.gameObject, networkConnection, targetScene);

            _createdPlayers.AddOrAppoint(networkConnection, instance);

            Created?.Invoke(instance, networkConnection);

            return instance;
        }

        public void TryRemovingPlayer(NetworkConnection networkConnection) => _createdPlayers.Remove(networkConnection);
    }
}