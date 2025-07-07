using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Managing.Client;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using Channel = FishNet.Transporting.Channel;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.Network
{
    public class ClientSetPlayersAnimationsPlacesSynchronizer
    {
        private SetPlayerAnimationUnprocessedEvent? _nearestSetAnimationEventSendersEvent;

        private readonly DeferredActionsInvoker _deferredActionsInvoker;
        private readonly EventBus _eventBus;
        private readonly ClientManager _clientManager;

        private readonly SetPlayerAnimationButtonControllersProviderService
            _setPlayerAnimationButtonControllersProviderService;

        public ClientSetPlayersAnimationsPlacesSynchronizer(EventBus eventBus, ClientManager clientManager,
            SetPlayerAnimationButtonControllersProviderService setPlayerAnimationButtonControllersProviderService,
            DeferredActionsInvoker deferredActionsInvoker)
        {
            _eventBus = eventBus;
            _clientManager = clientManager;
            _setPlayerAnimationButtonControllersProviderService = setPlayerAnimationButtonControllersProviderService;
            _deferredActionsInvoker = deferredActionsInvoker;
        }

        public void HandleSetPlayersAnimationsBroadcast(SetPlayersAnimationsBroadcastForClient broadcast,
            Channel channel) =>
            broadcast.Broadcasts.ForEach(broadcast => HandleSetPlayerAnimationBroadcastAsync(broadcast, channel));

        public void HandleSetBotsAnimationsBroadcast(SetBotsAnimationsBroadcastForClient broadcast, Channel channel)
            => broadcast.Broadcasts.ForEach(broadcast => HandleSetBotAnimationBroadcastAsync(broadcast, channel));

        public async void HandleSetPlayerAnimationBroadcastAsync(SetPlayerAnimationBroadcastForClient broadcast,
            Channel channel)
        {
            if (broadcast.PastBusySetAnimationSenderId.HasValue)
                _eventBus.Fire(new StoppedPlayerAnimationProcessedEvent(broadcast.PastBusySetAnimationSenderId.Value));

            if (!_setPlayerAnimationButtonControllersProviderService.ControllersById.TryGetValue(
                    broadcast.SetAnimationSenderId, out var controller))
            {
                await UniTask.WaitWhile(() =>
                    !_setPlayerAnimationButtonControllersProviderService.ControllersById.TryGetValue(
                        broadcast.SetAnimationSenderId, out controller));
            }

            SendSetPlayerAnimationProcessedEvent(controller, broadcast.ShouldStartAnimation, broadcast.ForOwner);
        }

        public async void HandleSetBotAnimationBroadcastAsync(SetBotAnimationBroadcastForClient broadcast,
            Channel channel)
        {
            if (broadcast.PastBusySetAnimationSenderId.HasValue)
                _eventBus.Fire(new StoppedPlayerAnimationProcessedEvent(broadcast.PastBusySetAnimationSenderId.Value));

            if (!_setPlayerAnimationButtonControllersProviderService.ControllersById.TryGetValue(
                    broadcast.SetAnimationSenderId, out var controller))
            {
                await UniTask.WaitWhile(() =>
                    !_setPlayerAnimationButtonControllersProviderService.ControllersById.TryGetValue(
                        broadcast.SetAnimationSenderId, out controller));
            }

            SendSetBotAnimationProcessedEvent(controller, broadcast.ShouldStartAnimation, broadcast.BotObjectId);
        }

        private void SendSetBotAnimationProcessedEvent(SetPlayerAnimationPlaceController controller,
            bool shouldStartAnimation, int botObjectId)
        {
            _eventBus.Fire(new SetBotAnimationEvent(shouldStartAnimation,
                controller.SerializableComponents.AnimationType,
                controller.SerializableComponents.PositionAndPlayerForwardDirectionProvider.forward, controller.Id, 
                botObjectId, controller.SerializableComponents.PositionAndPlayerForwardDirectionProvider.position));
        }

        private void SendSetPlayerAnimationProcessedEvent(SetPlayerAnimationPlaceController controller,
            bool shouldStartAnimation, bool forOwner)
        {
            _eventBus.Fire(new SetPlayerAnimationProcessedEvent(controller.SerializableComponents.AnimationType,
                controller.SerializableComponents.PositionAndPlayerForwardDirectionProvider.position, controller.Id,
                controller.SerializableComponents.PositionAndPlayerForwardDirectionProvider.forward,
                shouldStartAnimation, forOwner,
                controller.SerializableComponents.CharacterControllerHeight));
        }

        public void TrySettingNearestEventSenderAndBroadcastInEndOfFrame(
            Transform playerTransform, SetPlayerAnimationUnprocessedEvent unprocessedEvent)
        {
            if (_nearestSetAnimationEventSendersEvent.HasValue && Vector3.Distance(playerTransform.position,
                    _nearestSetAnimationEventSendersEvent.Value.Position) <=
                Vector3.Distance(playerTransform.position, unprocessedEvent.Position)) return;

            _nearestSetAnimationEventSendersEvent = unprocessedEvent;

            _deferredActionsInvoker.UpdateActionAndInvokeActionInEndOfFrame("PlayAnimation",
                () =>
                {
                    _clientManager.Broadcast(new SetPlayerAnimationBroadcastForServer(
                        _nearestSetAnimationEventSendersEvent.Value.SenderId));
                    _nearestSetAnimationEventSendersEvent = null;
                });
        }
    }
}