using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing;
using UnityEngine;
using WelwiseHubBotsModule.Runtime.Server.Scripts;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.PlayerSystem;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.GameStateMachinePart
{
    public class GameStateMachine
    {
        public event Action<IExitableGameState, NetworkConnection> EnteredInState;

        private readonly Dictionary<NetworkConnection, IExitableGameState> _activeGameStateByClientNetworkConnection =
            new Dictionary<NetworkConnection, IExitableGameState>();

        private readonly Dictionary<Type, IExitableGameState>
            _statesByType = new Dictionary<Type, IExitableGameState>();

        private readonly Dictionary<GameState, IExitableGameState> _statesByEnum =
            new Dictionary<GameState, IExitableGameState>();

        public GameStateMachine(ClientInitializationGameState clientInitializationGameState,
            HubGameState hubGameState, EventBus eventBus,
            NetworkManager networkManager,
            ClientsConnectionTrackingServiceForServer clientsConnectionTrackingServiceForServer,
            IAssetLoader assetLoader)
        {
            RegisterState(clientInitializationGameState, GameState.Initialization);
            RegisterState(hubGameState, GameState.Hub);

            eventBus.Subscribe<EnterServerStateEvent>(Enter);

            clientsConnectionTrackingServiceForServer.Disconnected += RemoveNetworkConnectionFromActiveGameStates;

            InitializeAsync(networkManager, assetLoader);
        }

        public void Enter<TState>(NetworkConnection networkConnection) where TState : class, IGameState
            => Enter(GetState<TState>(), networkConnection);

        private async void InitializeAsync(NetworkManager networkManager, IAssetLoader assetLoader)
        {
#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#endif

            await NetworkAssetsLoader.AddNetworkObjectsToSpawnableObjects();

            var settings = NetworkTools.LoadServerSettings();
            networkManager.TransportManager.Transport.SetPort((ushort)settings.Port);

            networkManager.ServerManager.StartConnection();
        }

        private void RemoveNetworkConnectionFromActiveGameStates(NetworkConnection networkConnection) =>
            _activeGameStateByClientNetworkConnection.Remove(networkConnection);

        private void Enter(EnterServerStateEvent @event) =>
            Enter(_statesByEnum[@event.GameState], @event.Connection, @event.Dependency);

        private void Enter<TState>(TState state, NetworkConnection networkConnection, object dependency = null)
            where TState : class, IExitableGameState
        {
            switch (state)
            {
                case null:
                    return;
                case IGameState gameState:
                    gameState.EnterAsync(networkConnection).Forget();
                    break;
                case ClientInitializationGameState clientInitializationGameState
                    when dependency is ClientData clientData:
                    clientInitializationGameState.EnterAsync(networkConnection, clientData).Forget();
                    break;
                default:
                    return;
            }

            _activeGameStateByClientNetworkConnection.GetValueOrDefault(networkConnection)
                ?.ExitAsync(networkConnection);

            EnteredInState?.Invoke(state, networkConnection);
            _activeGameStateByClientNetworkConnection.TryAdd(networkConnection, state);
        }

        private TState GetState<TState>() where TState : class, IGameState =>
            _statesByType[typeof(TState)] as TState;

        private void RegisterState<TState>(TState implementation, GameState? state = null)
            where TState : class, IExitableGameState
        {
            _statesByType.Add(typeof(TState), implementation);

            if (state != null)
                _statesByEnum.Add((GameState)state, implementation);
        }
    }
}