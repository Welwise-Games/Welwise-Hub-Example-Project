using System.Collections.Generic;
using FishNet.Connection;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    public interface IPlayersPetsViewControllerProviderService
    {
        IReadOnlyDictionary<NetworkConnection, PlayerPetsViewController> PlayersViewControllers { get; }
    }
}