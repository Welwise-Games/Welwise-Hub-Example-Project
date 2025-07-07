using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotBehaviourController
    {
        public bool IsInteracting { get; private set; }
        
        public event Action<bool> ChangedRunningState;

        public event Action EnteredPortal;
        public event Action<string> StartedPlayingEmotion;


        private SetPlayerAnimationPlaceModel _targetSetPlayerAnimationPlaceModel;
        public readonly SharedBotSerializableComponents SerializableComponents;

        private readonly BotBehaviourModel _botBehaviourModel;

        private readonly SetPlayerAnimationPlaceModelsProviderService _setPlayerAnimationPlaceModelsProviderService;
        private readonly Transform[] _portalsTransforms;
        private readonly IRoom _room;
        private readonly Transform _shopTransform;
        private readonly EmotionsConfigsProviderService _emotionsConfigsProviderService;
        private readonly ServerSetPlayersAnimationsPlacesSynchronizer _serverSetPlayersAnimationsPlacesSynchronizer;
        private readonly BotsNicknamesProviderService _botsNicknamesProviderService;
        private readonly BotsCustomizationDataProviderService _botsCustomizationDataProviderService;
        private readonly ClientsConfigsProviderService _clientsConfigsProviderService;
        private readonly ItemsConfigsProviderService _itemsConfigsProviderService;

        private readonly int _objectId;

        private const float MinimalValue = 0.1f;

        public BotBehaviourController(SharedBotSerializableComponents serializableComponents,
            BotBehaviourModel botBehaviourModel,
            SetPlayerAnimationPlaceModelsProviderService setPlayerAnimationPlaceModelsProviderService, IRoom room,
            Transform[] portalsTransforms, Transform shopTransform,
            EmotionsConfigsProviderService emotionsConfigsProviderService,
            ServerSetPlayersAnimationsPlacesSynchronizer serverSetPlayersAnimationsPlacesSynchronizer,
            BotsNicknamesProviderService botsNicknamesProviderService,
            BotsCustomizationDataProviderService botsCustomizationDataProviderService,
            ClientsConfigsProviderService clientsConfigsProviderService,
            ItemsConfigsProviderService itemsConfigsProviderService)
        {
            SerializableComponents = serializableComponents;
            _botBehaviourModel = botBehaviourModel;
            _setPlayerAnimationPlaceModelsProviderService = setPlayerAnimationPlaceModelsProviderService;
            _room = room;
            _portalsTransforms = portalsTransforms;
            _shopTransform = shopTransform;
            _emotionsConfigsProviderService = emotionsConfigsProviderService;
            _serverSetPlayersAnimationsPlacesSynchronizer = serverSetPlayersAnimationsPlacesSynchronizer;
            _botsNicknamesProviderService = botsNicknamesProviderService;
            _botsCustomizationDataProviderService = botsCustomizationDataProviderService;
            _clientsConfigsProviderService = clientsConfigsProviderService;
            _itemsConfigsProviderService = itemsConfigsProviderService;

            _objectId = SerializableComponents.GetComponent<NetworkObject>().ObjectId;

            _botBehaviourModel.UpdatedInterestPointGroup += GoToInterestPointAndTryInteracting;

            ChangedRunningState += SetIsStopped;

            void SetIsStopped(bool isRunning) => SerializableComponents.NavMeshAgent.isStopped = !isRunning;
                
            _botBehaviourModel.EndedPlayingEmotionTimer += async () =>
            {
                if (IsInteracting && botBehaviourModel.TargetInterestPointGroup == InterestPointGroup.Portals)
                    return;

                if (!IsInteracting)
                    ChangedRunningState?.Invoke(false);

                await PlayRandomEmotionAnimationAsync();

                if (!IsInteracting)
                    ChangedRunningState?.Invoke(true);

                _botBehaviourModel.TryPlayingEmotionTimer();
            };

            GoToInterestPointAndTryInteracting(botBehaviourModel.TargetInterestPointGroup);
        }

        public void Dispose()
        {
            EnteredPortal = null;
            StartedPlayingEmotion = null;
        }

        private Vector3? GetDestinationPosition(InterestPointGroup group)
        {
            switch (group)
            {
                case InterestPointGroup.Bar:
                    var place = _setPlayerAnimationPlaceModelsProviderService.ModelsByRoom.GetValueOrDefault(_room)
                        ?.GetRandomOrDefault();
                    _targetSetPlayerAnimationPlaceModel = place;
                    return place?.Position;
                case InterestPointGroup.Portals:
                    return _portalsTransforms.GetRandomOrDefault()?.position;
                case InterestPointGroup.Shop:
                    return _shopTransform.transform.position;
                default:
                    return null;
            }
        }

        private async void InteractWithInterestPointAsync(InterestPointGroup group)
        {
            switch (group)
            {
                case InterestPointGroup.Portals:
                    SerializableComponents.NavMeshAgent.stoppingDistance = MinimalValue;

                    await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.WaitWhile(() =>
                            SerializableComponents.NavMeshAgent.remainingDistance >
                            SerializableComponents.NavMeshAgent.stoppingDistance,
                        cancellationToken: SerializableComponents.destroyCancellationToken));

                    if (!SerializableComponents)
                        return;

                    EnteredPortal?.Invoke();
                    break;
                case InterestPointGroup.Bar:
                    SerializableComponents.NavMeshAgent.enabled = false;
                    SerializableComponents.NetworkTransform.SetSynchronizePosition(false);
                    SerializableComponents.NetworkTransform.SetSynchronizeRotation(false);

                    await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.Delay(TimeSpan.FromSeconds(MinimalValue),
                        cancellationToken: SerializableComponents.destroyCancellationToken));

                    if (!SerializableComponents)
                        return;

                    _serverSetPlayersAnimationsPlacesSynchronizer.TryHandlingSettingBotAnimation(_objectId,
                        _room, _targetSetPlayerAnimationPlaceModel, true, out var succesfully);
                    break;
                case InterestPointGroup.Shop:
                {
                    var stoppingDistance = SerializableComponents.NavMeshAgent.stoppingDistance;

                    SerializableComponents.NavMeshAgent.stoppingDistance = MinimalValue;

                    ChangedRunningState?.Invoke(true);

                    await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.WaitWhile(() =>
                            SerializableComponents.NavMeshAgent.remainingDistance >
                            SerializableComponents.NavMeshAgent.stoppingDistance,
                        cancellationToken: SerializableComponents.destroyCancellationToken));

                    if (!SerializableComponents)
                        return;

                    SerializableComponents.NavMeshAgent.stoppingDistance = stoppingDistance;

                    ChangedRunningState?.Invoke(false);
                    break;
                }
            }
        }

        private async void GoToInterestPointAndTryInteracting(InterestPointGroup group)
        {
            if (_targetSetPlayerAnimationPlaceModel != null)
            {
                SerializableComponents.NavMeshAgent.enabled = true;
                SerializableComponents.NetworkTransform.SetSynchronizePosition(true);
                SerializableComponents.NetworkTransform.SetSynchronizeRotation(true);

                await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.Delay(TimeSpan.FromSeconds(MinimalValue),
                    cancellationToken: SerializableComponents.destroyCancellationToken));

                if (!SerializableComponents)
                    return;

                _serverSetPlayersAnimationsPlacesSynchronizer.TryHandlingSettingBotAnimation(_objectId,
                    _room, _targetSetPlayerAnimationPlaceModel, false, out var succesfully);

                _targetSetPlayerAnimationPlaceModel = null;
            }
            else if (IsInteracting)
            {
                if (_botBehaviourModel.ShouldSetCustomizationDataPart())
                {
                    var myNickname = _botsNicknamesProviderService.Nicknames[_objectId];
                    var newNickname = BotsNicknamesTools.GetRandomNickname(myNickname);
                    _botsNicknamesProviderService.TrySettingBotNickname(_objectId, newNickname);
                }

                _botsCustomizationDataProviderService.TrySettingBotCustomizationData(_objectId,
                    BotsCustomizationDataTools.GetRandomCustomizationData(
                        _botsCustomizationDataProviderService.BotsCustomizationData[_objectId],
                        await _clientsConfigsProviderService.GetClientsConfigAsync(),
                        _botBehaviourModel.GetSetDataPartChance(),
                        await _itemsConfigsProviderService.GetItemsConfigAsync()));
            }

            IsInteracting = false;

            var position = GetDestinationPosition(group);

            if (!position.HasValue)
            {
                _botBehaviourModel.UpdateInterestPointGroup();
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

            var shouldInteractWithLastInterestPoint = _botBehaviourModel.ShouldInteractWithLastInterestPoint();

            if (group != InterestPointGroup.Portals || !shouldInteractWithLastInterestPoint)
            {
                _botBehaviourModel.StartChangingInterestPointTimer();

                ChangedRunningState?.Invoke(false);
            }

            if (shouldInteractWithLastInterestPoint)
                InteractWithInterestPointAsync(group);

            IsInteracting = true;
        }

        private async UniTask PlayRandomEmotionAnimationAsync()
        {
            var config = (await _emotionsConfigsProviderService.GetEmotionsAnimationsConfig())
                .EmotionsAnimationConfigs
                .GetRandomOrDefault();

            if (config == null)
                _botBehaviourModel.TryPlayingEmotionTimer();
            else
            {
                StartedPlayingEmotion?.Invoke(config.EmotionIndex);
                await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.Delay(TimeSpan.FromSeconds(config.AnimationLength),
                    cancellationToken: SerializableComponents.destroyCancellationToken));
            }
        }
    }
}