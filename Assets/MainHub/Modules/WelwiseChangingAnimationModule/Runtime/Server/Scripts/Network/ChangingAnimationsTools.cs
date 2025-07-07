using System;
using System.Collections.Generic;
using FishNet.Managing.Server;
using FishNet.Managing.Timing;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts;
using WelwiseSharedModule.Runtime.Server.Scripts;

namespace WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network
{
    public static class ChangingAnimationsTools
    {
        public static void Initialize(IRoomsProviderService roomsProviderService,
            Func<IRoom, HashSet<SetPlayerAnimationPlaceModel>> getNewButtonsModelsByRoomFunc,
            ServerManager serverManager,
            out ChangingAnimationsEntryPointData entryPointData,
            SetPlayerAnimationPlaceModelsProviderService setPlayerAnimationButtonModelsProviderService = null)
        {
            setPlayerAnimationButtonModelsProviderService ??= new SetPlayerAnimationPlaceModelsProviderService();

            var synchronizer =
                new ServerSetPlayersAnimationsPlacesSynchronizer(setPlayerAnimationButtonModelsProviderService,
                    serverManager, roomsProviderService);

            roomsProviderService.RoomRemoved += setPlayerAnimationButtonModelsProviderService.RemoveModelsByRoom;

            roomsProviderService.RoomCreated += room => setPlayerAnimationButtonModelsProviderService.AddModelsByRoom(
                room, getNewButtonsModelsByRoomFunc.Invoke(room));

            roomsProviderService.ClientConnectedToRoom += (networkConnection, room) =>
                synchronizer.SendBroadcastForInitializeRoomButtonsModelsForConnectedClient(networkConnection);

            roomsProviderService.ClientDisconnectedFromRoom +=
                synchronizer.TrySendingAnimationBroadcastOnClientDisconnect;

            serverManager.RegisterBroadcast<SetPlayerAnimationBroadcastForServer>(synchronizer
                .HandleSetPlayerAnimationBroadcast);

            entryPointData = new ChangingAnimationsEntryPointData(synchronizer);
        }
    }
}