using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BarInterestPointGroupInteractor : IInterestPointGroupInteractor
    {
        public bool IsDestroyInteractionAction => false;

        private SetPlayerAnimationPlaceModel _targetSetPlayerAnimationPlaceModel;

        private readonly SharedBotSerializableComponents _botSerializableComponents;
        private readonly SetPlayerAnimationPlaceModelsProviderService _setPlayerAnimationPlaceModelsProviderService;
        private readonly ServerSetPlayersAnimationsPlacesSynchronizer _serverSetPlayersAnimationsPlacesSynchronizer;
        private readonly IRoom _room;

        private readonly int _objectId;
        private readonly float _minimalValue;


        public BarInterestPointGroupInteractor(SharedBotSerializableComponents botSerializableComponents,
            SetPlayerAnimationPlaceModelsProviderService setPlayerAnimationPlaceModelsProviderService,
            ServerSetPlayersAnimationsPlacesSynchronizer serverSetPlayersAnimationsPlacesSynchronizer, IRoom room,
            int objectId, float minimalValue)
        {
            _botSerializableComponents = botSerializableComponents;
            _setPlayerAnimationPlaceModelsProviderService = setPlayerAnimationPlaceModelsProviderService;
            _serverSetPlayersAnimationsPlacesSynchronizer = serverSetPlayersAnimationsPlacesSynchronizer;
            _room = room;
            _objectId = objectId;
            _minimalValue = minimalValue;
        }

        public void Dispose()
        {
        }

        public Vector3? GetDestinationPosition() =>
            (_targetSetPlayerAnimationPlaceModel = _setPlayerAnimationPlaceModelsProviderService.ModelsByRoom
                .GetValueOrDefault(_room)
                ?.GetRandomOrDefault())?.Position;

        public async UniTask StartInteractionWithInterestPointAsync(Action<bool> changedRunningState)
        {
            _botSerializableComponents.NavMeshAgent.enabled = false;
            _botSerializableComponents.NetworkTransform.SetSynchronizePosition(false);
            _botSerializableComponents.NetworkTransform.SetSynchronizeRotation(false);

            await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.Delay(
                TimeSpan.FromSeconds(_minimalValue),
                cancellationToken: _botSerializableComponents.destroyCancellationToken));

            if (!_botSerializableComponents)
                return;

            _serverSetPlayersAnimationsPlacesSynchronizer.TryHandlingSettingBotAnimation(_objectId,
                _room, _targetSetPlayerAnimationPlaceModel, true, out var succesfully);
        }

        public async UniTask OnEndInteraction()
        {
            if (_targetSetPlayerAnimationPlaceModel == null)
                return;

            _botSerializableComponents.NavMeshAgent.enabled = true;
            _botSerializableComponents.NetworkTransform.SetSynchronizePosition(true);
            _botSerializableComponents.NetworkTransform.SetSynchronizeRotation(true);

            await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.Delay(
                TimeSpan.FromSeconds(_minimalValue),
                cancellationToken: _botSerializableComponents.destroyCancellationToken));

            if (!_botSerializableComponents)
                return;

            _serverSetPlayersAnimationsPlacesSynchronizer.TryHandlingSettingBotAnimation(_objectId,
                _room, _targetSetPlayerAnimationPlaceModel, false, out var succesfully);

            _targetSetPlayerAnimationPlaceModel = null;
        }
    }
}