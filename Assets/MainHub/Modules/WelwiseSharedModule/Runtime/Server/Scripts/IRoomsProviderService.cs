using System;
using System.Collections.Generic;
using FishNet.Connection;

namespace WelwiseSharedModule.Runtime.Server.Scripts
{
    public interface IRoomsProviderService
    {
        IReadOnlyDictionary<NetworkConnection, IRoom> RoomsByConnectedClientsNetworkConnections { get; }
        event Action<IRoom> RoomRemoved, RoomCreated;
        event Action<NetworkConnection, IRoom> ClientConnectedToRoom, ClientDisconnectedFromRoom;
    }
}