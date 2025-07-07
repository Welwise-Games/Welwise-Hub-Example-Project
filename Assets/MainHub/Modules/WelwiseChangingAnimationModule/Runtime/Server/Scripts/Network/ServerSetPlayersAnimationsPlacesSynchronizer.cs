using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Managing.Timing;
using FishNet.Transporting;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network
{
    public class ServerSetPlayersAnimationsPlacesSynchronizer
    {
        private readonly SetPlayerAnimationPlaceModelsProviderService _setPlayerAnimationPlaceModelsProviderService;
        private readonly ServerManager _serverManager;
        private readonly IRoomsProviderService _roomsProviderService;

        public ServerSetPlayersAnimationsPlacesSynchronizer(
            SetPlayerAnimationPlaceModelsProviderService setPlayerAnimationPlaceModelsProviderService,
            ServerManager serverManager, IRoomsProviderService roomsProviderService)
        {
            _setPlayerAnimationPlaceModelsProviderService = setPlayerAnimationPlaceModelsProviderService;
            _serverManager = serverManager;
            _roomsProviderService = roomsProviderService;
        }

        public void SendBroadcastForInitializeRoomButtonsModelsForConnectedClient(NetworkConnection networkConnection)
        {
            if (!_roomsProviderService.RoomsByConnectedClientsNetworkConnections.TryGetValue(networkConnection,
                    out var room) ||
                !_setPlayerAnimationPlaceModelsProviderService.ModelsByRoom.TryGetValue(room, out var models))
                return;

            var busyModels = models.Where(model => model.IsPlaceBusy).ToList();

            if (busyModels.Count == 0)
                return;

            var playersBroadcast = new SetPlayersAnimationsBroadcastForClient(
                busyModels.Where(model => !model.IsPlaceOwnerBot).Select(model =>
                    new SetPlayerAnimationBroadcastForClient(model.Id, null, true,
                        false)).ToList());

            var botsBroadcast = new SetBotsAnimationsBroadcastForClient(
                busyModels.Where(model => model.IsPlaceOwnerBot).Select(model =>
                    new SetBotAnimationBroadcastForClient(model.Id, null, true, model.PlaceOwnerBotObjectId.Value)).ToList());

            _serverManager.Broadcast(networkConnection, botsBroadcast);
            _serverManager.Broadcast(networkConnection, playersBroadcast);
        }

        public void TrySendingAnimationBroadcastOnClientDisconnect(NetworkConnection networkConnection, IRoom room)
        {
            if (room.ConnectedClientsNetworkConnections.Count <= 0 || !_setPlayerAnimationPlaceModelsProviderService
                    .ModelsByRoom
                    .TryGetValue(room, out var models)) return;

            var busyModel = models.FirstOrDefault(model => model.PlaceOwnerPlayerConnection == networkConnection);

            if (busyModel == null)
                return;

            SendSetPlayerAnimationBroadcastForClients(networkConnection, room.ConnectedClientsNetworkConnections,
                busyModel.Id, null,
                false);
        }

        public void HandleSetPlayerAnimationBroadcast(NetworkConnection networkConnection,
            SetPlayerAnimationBroadcastForServer broadcast, Channel channel)
        {
            if (!_roomsProviderService.RoomsByConnectedClientsNetworkConnections.TryGetValue(networkConnection,
                    out var room) ||
                !_setPlayerAnimationPlaceModelsProviderService.ModelsByRoom.TryGetValue(room, out var models))
                return;

            var model = models.FirstOrDefault(model => model.Id == broadcast.SenderId);

            if (model == null)
                return;

            var shouldStartAnimation = !model.IsPlaceBusy;

            if (!shouldStartAnimation && model.PlaceOwnerPlayerConnection != networkConnection)
                return;

            int? pastBusySetAnimationSenderId = null;

            if (shouldStartAnimation)
            {
                var pastBusySetAnimationSender =
                    models.FirstOrDefault(model => model.PlaceOwnerPlayerConnection == networkConnection);
                pastBusySetAnimationSender?.SetPlaceOwnerPlayer(null);
                pastBusySetAnimationSenderId = pastBusySetAnimationSender?.Id;

                model.SetPlaceOwnerPlayer(networkConnection);
            }
            else
                model.SetPlaceOwnerPlayer(null);

            SendSetPlayerAnimationBroadcastForClients(networkConnection, room.ConnectedClientsNetworkConnections,
                model.Id,
                pastBusySetAnimationSenderId, shouldStartAnimation);
        }

        public void TryHandlingSettingBotAnimation(int botObjectId, IRoom room,
            SetPlayerAnimationPlaceModel placeModel, bool shouldStartAnimation,
            out bool succesfully)
        {
            succesfully = false;

            if (!_setPlayerAnimationPlaceModelsProviderService.ModelsByRoom.TryGetValue(room, out var models))
                return;

            int? pastBusySetAnimationSenderId = null;

            switch (shouldStartAnimation)
            {
                case true when placeModel.IsPlaceBusy: 
                    return;
                case true:
                {
                    var pastBusySetAnimationSender =
                        models.FirstOrDefault(model => model.PlaceOwnerBotObjectId == botObjectId);
                    pastBusySetAnimationSender?.UpdatePlaceOwnerBotObjectId(null);
                    pastBusySetAnimationSenderId = pastBusySetAnimationSender?.Id;
                    break;
                }
            }

            placeModel.UpdatePlaceOwnerBotObjectId(shouldStartAnimation ? botObjectId : null);

            SendSetBotAnimationBroadcastForClients(room.ConnectedClientsNetworkConnections, placeModel.Id,
                pastBusySetAnimationSenderId, shouldStartAnimation, botObjectId);

            succesfully = true;
        }

        private void SendSetBotAnimationBroadcastForClients(IReadOnlyCollection<NetworkConnection> clients,
            int setAnimationSenderId, int? pastBusySetAnimationSenderId, bool shouldStartAnimation, int botObjectId)
        {
            clients.ForEach(connection => _serverManager.Broadcast(connection, new
                SetBotAnimationBroadcastForClient(setAnimationSenderId,
                    pastBusySetAnimationSenderId, shouldStartAnimation, botObjectId)));
        }

        private void SendSetPlayerAnimationBroadcastForClients(NetworkConnection senderNetworkConnection,
            IReadOnlyCollection<NetworkConnection> clients,
            int setAnimationSenderId, int? pastBusySetAnimationSenderId, bool shouldStartAnimation)
        {
            clients.ForEach(connection => _serverManager.Broadcast(connection, new
                SetPlayerAnimationBroadcastForClient(setAnimationSenderId, pastBusySetAnimationSenderId,
                    shouldStartAnimation, connection == senderNetworkConnection)));
        }
    }
}