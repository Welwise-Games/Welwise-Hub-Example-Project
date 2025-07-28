using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class PortalsInteractPointGroupInteractor : IInterestPointGroupInteractor
    {
        private readonly SharedBotSerializableComponents _botSerializableComponents;
        private readonly float _minimalValue;
        private readonly Transform[] _portalsTransforms;

        public PortalsInteractPointGroupInteractor(SharedBotSerializableComponents botSerializableComponents,
            float minimalValue, Transform[] portalsTransforms)
        {
            _botSerializableComponents = botSerializableComponents;
            _minimalValue = minimalValue;
            _portalsTransforms = portalsTransforms;
        }

        public event Action EnteredPortal;

        public bool IsDestroyInteractionAction => true;

        public void Dispose()
        {
            EnteredPortal = null;
        }

        public Vector3? GetDestinationPosition() => _portalsTransforms.GetRandomOrDefault()?.position;

        public async UniTask StartInteractionWithInterestPointAsync(Action<bool> changedRunningState)
        {
            _botSerializableComponents.NavMeshAgent.stoppingDistance = _minimalValue;
            
            await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.WaitWhile(() =>
                    _botSerializableComponents.NavMeshAgent.remainingDistance >
                    _botSerializableComponents.NavMeshAgent.stoppingDistance,
                cancellationToken: _botSerializableComponents.destroyCancellationToken));

            if (!_botSerializableComponents)
                return;
            
            changedRunningState?.Invoke(false);

            EnteredPortal?.Invoke();
        }

        public UniTask OnEndInteraction()
        {
            return UniTask.CompletedTask;
        }
    }
}