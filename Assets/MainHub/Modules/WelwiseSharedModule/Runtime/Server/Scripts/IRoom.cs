using System;
using System.Collections.Generic;
using FishNet.Connection;

namespace WelwiseSharedModule.Runtime.Server.Scripts
{
    public interface IRoom
    {
        IReadOnlyCollection<NetworkConnection> ConnectedClientsNetworkConnections { get; }
    }
}