using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotController
    {
        public IReadOnlyDictionary<InterestPointGroup, IInterestPointGroupInteractor> InterestPointGroupInteractors =>
            _interestPointGroupDataInteractors;
        
        public bool IsInteracting { get; private set; }
        public event Action<bool> ChangedRunningState;
        public event Action<string> StartedPlayingEmotion;
        
        public readonly SharedBotSerializableComponents SerializableComponents;

        private readonly BotModel _botModel;

        private bool _isFirstInteraction;
        
        private readonly EmotionsConfigProviderService _emotionsConfigProviderService;

        private readonly Dictionary<InterestPointGroup, IInterestPointGroupInteractor>
            _interestPointGroupDataInteractors;

        public BotController(SharedBotSerializableComponents serializableComponents,
            BotModel botModel,
            EmotionsConfigProviderService emotionsConfigProviderService,
            Dictionary<InterestPointGroup, IInterestPointGroupInteractor> interestPointGroupDataInteractors)
        {
            SerializableComponents = serializableComponents;
            _botModel = botModel;
            _emotionsConfigProviderService = emotionsConfigProviderService;
            _interestPointGroupDataInteractors = interestPointGroupDataInteractors;

            _botModel.UpdatedInterestPointGroup += GoToInterestPointAndTryInteracting;

            ChangedRunningState += SetIsStopped;

            void SetIsStopped(bool isRunning) => SerializableComponents.NavMeshAgent.isStopped = !isRunning;

            _botModel.EndedPlayingEmotionTimer += async () =>
            {
                if (IsInteracting && botModel.TargetInterestPointGroup == InterestPointGroup.Portals)
                    return;

                if (!IsInteracting)
                    ChangedRunningState?.Invoke(false);

                await PlayRandomEmotionAnimationAsync();

                if (!IsInteracting)
                    ChangedRunningState?.Invoke(true);

                _botModel.TryPlayingEmotionTimer();
            };

            GoToInterestPointAndTryInteracting(botModel.TargetInterestPointGroup);
        }

        public void Dispose()
        {
            _interestPointGroupDataInteractors.ForEach(provider => provider.Value.Dispose());
            StartedPlayingEmotion = null;
        }

        private Vector3? GetDestinationPosition(InterestPointGroup group) =>
            _interestPointGroupDataInteractors.GetValueOrDefault(group)?.GetDestinationPosition();

        private async void StartInteractionWithInterestPointAsync(InterestPointGroup group)
        {
            var interestPointGroupDataProvider = _interestPointGroupDataInteractors.GetValueOrDefault(group);

            if (interestPointGroupDataProvider != null)
                await interestPointGroupDataProvider.StartInteractionWithInterestPointAsync(ChangedRunningState);
        }

        private async void GoToInterestPointAndTryInteracting(InterestPointGroup group)
        {
            if (IsInteracting)
            {
                await _interestPointGroupDataInteractors.GetValueOrDefault(_botModel.PastTargetInterestPointGroup)
                    .OnEndInteraction();
            }
            
            IsInteracting = false;

            var position = GetDestinationPosition(group);

            if (!position.HasValue)
            {
                _botModel.UpdateInterestPointGroup();
                return;
            }

            SerializableComponents.NavMeshAgent.SetDestination(position.Value);

            ChangedRunningState?.Invoke(true);

            await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.WaitWhile(() =>
                    SerializableComponents.NavMeshAgent.pathPending ||
                    SerializableComponents.NavMeshAgent.remainingDistance >
                    SerializableComponents.NavMeshAgent.stoppingDistance,
                cancellationToken: SerializableComponents.destroyCancellationToken));

            if (!SerializableComponents)
                return;

            var shouldInteractWithLastInterestPoint = _botModel.ShouldInteractWithLastInterestPoint();

            if (!_interestPointGroupDataInteractors.GetValueOrDefault(group).IsDestroyInteractionAction || !shouldInteractWithLastInterestPoint)
            {
                _botModel.StartChangingInterestPointTimer();
                ChangedRunningState?.Invoke(false);
            }

            if (shouldInteractWithLastInterestPoint)
                StartInteractionWithInterestPointAsync(group);

            IsInteracting = true;
        }

        private async UniTask PlayRandomEmotionAnimationAsync()
        {
            var config = (await _emotionsConfigProviderService.GetEmotionsAnimationsConfig())
                .Configs
                .GetRandomOrDefault();

            if (config == null)
                _botModel.TryPlayingEmotionTimer();
            else
            {
                StartedPlayingEmotion?.Invoke(config.Index);
                await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.Delay(
                    TimeSpan.FromSeconds(config.AnimationLength),
                    cancellationToken: SerializableComponents.destroyCancellationToken));
            }
        }
    }
}