using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Managing.Server;
using UnityEngine;
using UnityEngine.SceneManagement;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Holders;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.HubSystem;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem
{
    public class HubsProviderService : IRoomsProviderService
    {
        public IReadOnlyDictionary<NetworkConnection, IRoom> RoomsByConnectedClientsNetworkConnections =>
            _hubByPlayerNetworkConnection.ToDictionary(pair => pair.Key, pair => pair.Value as IRoom);

        public IReadOnlyDictionary<NetworkConnection, Hub> HubByPlayerNetworkConnection =>
            _hubByPlayerNetworkConnection;

        public bool DoesHaveFreeHub => FreeHub != null;
        private Hub FreeHub => _hubs.FirstOrDefault(hub => hub.Value.PeopleCount < _playerLimitPerHub).Value;

        private bool _doesLoadHub;

        public event Action<Hub> HubRemoved, HubCreated;
        public event Action<NetworkConnection, Hub> ClientConnectedToHub, ClientDisconnectedFromHub;

        public event Action<IRoom> RoomRemoved
        {
            add => _roomRemoved += value;
            remove => _roomRemoved -= value;
        }

        public event Action<IRoom> RoomCreated
        {
            add => _roomCreated += value;
            remove => _roomCreated -= value;
        }

        public event Action<NetworkConnection, IRoom> ClientConnectedToRoom
        {
            add => _clientConnectedToRoom += value;
            remove => _clientConnectedToRoom -= value;
        }

        public event Action<NetworkConnection, IRoom> ClientDisconnectedFromRoom
        {
            add => _clientDisconnectedFromRoom += value;
            remove => _clientDisconnectedFromRoom -= value;
        }

        private Action<IRoom> _roomRemoved, _roomCreated;
        private Action<NetworkConnection, IRoom> _clientConnectedToRoom, _clientDisconnectedFromRoom;

        private int _currentHubIndex;

        private readonly int _playerLimitPerHub;
        private readonly Dictionary<int, Hub> _hubs = new Dictionary<int, Hub>();

        private readonly Dictionary<NetworkConnection, Hub> _hubByPlayerNetworkConnection =
            new Dictionary<NetworkConnection, Hub>();

        private readonly ServerManager _serverManager;
        private readonly FishNet.Managing.Scened.SceneManager _sceneManager;
        private readonly Container _container = new Container();
        private readonly IAssetLoader _assetLoader;


        public const string HubAssetId =
#if ADDRESSABLES
        "Hub";
#else
        "WelwiseHubExampleModule/Runtime/Shared/Prefabs/Hub";
#endif

        public HubsProviderService(ServerManager serverManager, FishNet.Managing.Scened.SceneManager sceneManager,
            IAssetLoader assetLoader, int playerLimitPerHub = 20)
        {
            _serverManager = serverManager;
            _sceneManager = sceneManager;
            _assetLoader = assetLoader;
            _playerLimitPerHub = playerLimitPerHub;
        }

        public void TryDisconnectingClientFromHub(NetworkConnection conn)
        {
            if (!_hubByPlayerNetworkConnection.Remove(conn, out var hub)) return;
            
            hub.RemoveClient(conn);

            ClientDisconnectedFromHub?.Invoke(conn, hub);
            _clientDisconnectedFromRoom?.Invoke(conn, hub);

            if (hub.PeopleCount == 0)
                UnloadHub(hub);
        }

        public async UniTask<Hub> TryGettingAndAddingPlayerToFreeHubAsync(NetworkConnection conn,
            DataContainer<Scene> playerSceneContainer)
        {
            var freeHub = FreeHub;

            var hub = freeHub ?? await GetCreatedHubAsync();

            ConnectClientToHub(conn, hub);
            playerSceneContainer.Data = hub.SceneLoadData.GetFirstLookupScene();

            return hub;
        }

        public async UniTask<Hub> GetCreatedHubAsync()
        {
            if (_doesLoadHub)
            {
                await UniTask.WaitWhile(() => !DoesHaveFreeHub && !_doesLoadHub);
                return FreeHub ?? await GetCreatedHubAsync();
            }

            _doesLoadHub = true;

            var newHubId = _hubs.Count + 1;

            var scene = SceneManager.LoadScene(ScenesNames.Hub,
                new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));

            var sceneLoadData = new SceneLoadData(scene)
                { Options = new LoadOptions { LocalPhysics = LocalPhysicsMode.Physics3D, AllowStacking = true } };

            var hubPrefab =
                await _container.GetOrLoadAndRegisterObjectAsync<SharedHubSerializableComponents>(HubAssetId,
                    _assetLoader, shouldCreate: false);

            var hubInstance = UnityEngine.Object.Instantiate(hubPrefab);

            SceneManager.MoveGameObjectToScene(hubInstance.gameObject, sceneLoadData.GetFirstLookupScene());
            _serverManager.Spawn(hubInstance.gameObject, scene: sceneLoadData.GetFirstLookupScene());

            var newHub = new Hub(newHubId, sceneLoadData, hubInstance);

            _hubs.Add(newHubId, newHub);

            HubCreated?.Invoke(newHub);
            _roomCreated?.Invoke(newHub);

            _doesLoadHub = false;

            return newHub;
        }

        private void UnloadHub(Hub hub)
        {
            _hubs.Remove(hub.Id);

            HubRemoved?.Invoke(hub);
            _roomRemoved?.Invoke(hub);

            var hud = new SceneUnloadData(hub.SceneLoadData.GetFirstLookupScene().handle);
            _sceneManager.UnloadConnectionScenes(hud);
        }

        private void ConnectClientToHub(NetworkConnection conn, Hub hub)
        {
            _sceneManager.LoadConnectionScenes(conn, hub.SceneLoadData);

            hub.AddClient(conn);
            _hubByPlayerNetworkConnection.Add(conn, hub);

            ClientConnectedToHub?.Invoke(conn, hub);
            _clientConnectedToRoom?.Invoke(conn, hub);
        }
    }
}