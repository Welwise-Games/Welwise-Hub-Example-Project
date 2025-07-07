using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public class GameStateMachine
    {
        public IExitableGameState ActiveGameState { get; private set; }

        private readonly Dictionary<GameState, IExitableGameState> _statesByEnum =
            new Dictionary<GameState, IExitableGameState>();

        private readonly Dictionary<Type, IExitableGameState>
            _statesByType = new Dictionary<Type, IExitableGameState>();

        private readonly IAssetLoader _assetLoader;

        public GameStateMachine(BootstrapGameState bootstrapGameState, InitializationGameState initializationGameState,
            HubGameState hubState, ReconnectionGameState reconnectionGameState, EventBus eventBus, IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
            RegisterState(bootstrapGameState);
            RegisterState(initializationGameState, GameState.Initialization);
            RegisterState(hubState, GameState.Hub);
            RegisterState(reconnectionGameState, GameState.Reconnection);
            InitializeAsync();

            eventBus.Subscribe<EnterClientStateEvent>(@event => TryEnteringStateAsync(@event).Forget());
        }

        private async void InitializeAsync()
        {
            await NetworkAssetsLoader.AddNetworkObjectsToSpawnableObjects();
            await TryEnteringStateAsync<BootstrapGameState>();
            await TryEnteringStateAsync<InitializationGameState>();
        }

        public async UniTask TryEnteringStateAsync<TState>() where TState : class, IGameState
            => await TryEnteringStateAsync(GetState<TState>());

        private async UniTask TryChangingStateAsync<TState>(TState state)
            where TState : class, IExitableGameState
        {
            if (state == null)
                return;

            if (ActiveGameState != null)
                await ActiveGameState.ExitAsync();
            
            ActiveGameState = state;
        }

        private async UniTask TryEnteringStateAsync<TState>(TState state)
            where TState : class, IGameState
        {
            if (state == null)
                return;
            
            await TryChangingStateAsync(state);
            await state.EnterAsync();
        }

        private async UniTask TryEnteringStateAsync(EnterClientStateEvent @event) =>
            await TryEnteringStateAsync(_statesByEnum[@event.GameState] as IGameState);

        private TState GetState<TState>() where TState : class, IGameState =>
            _statesByType.GetValueOrDefault(typeof(TState)) as TState;

        private void RegisterState<TState>(TState implementation, GameState? state = null)
            where TState : class, IExitableGameState
        {
            _statesByType.Add(typeof(TState), implementation);

            if (state != null)
                _statesByEnum.Add((GameState)state, implementation);
        }
    }
}