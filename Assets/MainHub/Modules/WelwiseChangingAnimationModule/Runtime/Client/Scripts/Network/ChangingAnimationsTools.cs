using System;
using System.Linq;
using FishNet.Managing.Client;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.Network
{
    public static class ChangingAnimationsTools
    {
        public static void Initialize(Func<Transform> playerTransformFunc,
            EventBus eventBus, ClientManager clientManager,
            ClientsConnectionTrackingServiceForClient connectionTrackingService,
            out ChangingAnimationsDataFromInitialize changingAnimationsDataFromInitialize,
            MonoBehaviour coroutineRunner)
        {
            var setPlayerAnimationButtonControllersProviderService =
                new SetPlayerAnimationButtonControllersProviderService();

            var synchronizer = new ClientSetPlayersAnimationsPlacesSynchronizer(eventBus, clientManager,
                setPlayerAnimationButtonControllersProviderService, new DeferredActionsInvoker(new DeferredActionInvoker(coroutineRunner)));

            eventBus.Subscribe<SetPlayerAnimationUnprocessedEvent>(@event =>
                synchronizer.TrySettingNearestEventSenderAndBroadcastInEndOfFrame(playerTransformFunc.Invoke(),
                    @event));

            connectionTrackingService.OwnerDisconnected +=
                setPlayerAnimationButtonControllersProviderService.ClearControllers;

            eventBus.Subscribe<StopPlayerAnimationUnprocessedEvent>(@event =>
                clientManager.Broadcast(new SetPlayerAnimationBroadcastForServer(@event.SenderId)));

            clientManager.RegisterBroadcast<SetPlayerAnimationBroadcastForClient>(synchronizer
                .HandleSetPlayerAnimationBroadcastAsync);
                
            clientManager.RegisterBroadcast<SetBotAnimationBroadcastForClient>(synchronizer
                .HandleSetBotAnimationBroadcastAsync);

            clientManager.RegisterBroadcast<SetPlayersAnimationsBroadcastForClient>(synchronizer
                .HandleSetPlayersAnimationsBroadcast);
            
            clientManager.RegisterBroadcast<SetBotsAnimationsBroadcastForClient>(synchronizer
                .HandleSetBotsAnimationsBroadcast);

            changingAnimationsDataFromInitialize =
                new ChangingAnimationsDataFromInitialize(setPlayerAnimationButtonControllersProviderService);
        }

        public static void CreateAndAppointSetPlayerAnimationAndPositionButtonControllers(Camera camera,
            SetPlayerAnimationPlaceSerializableComponents[] components,
            SetPlayerAnimationsButtonsConfig config, EventBus eventBus,
            SetPlayerAnimationButtonControllersProviderService
                setPlayerAnimationButtonControllersProviderService)
        {
            setPlayerAnimationButtonControllersProviderService.AppointControllers(Enumerable.Range(0, components.Length)
                .Select(i =>
                    new SetPlayerAnimationPlaceController(
                        components[i], eventBus, config,
                        camera, i)).ToHashSet());
        }
    }
}