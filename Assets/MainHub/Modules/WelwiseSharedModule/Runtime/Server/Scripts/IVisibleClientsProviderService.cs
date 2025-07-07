using System.Collections.Generic;
using FishNet.Connection;

namespace WelwiseSharedModule.Runtime.Server.Scripts
{
    public interface IVisibleClientsProviderService
    {
        IReadOnlyCollection<NetworkConnection> GetVisibleClientsForClient(NetworkConnection networkConnection);
    }
}