using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using WelwisePetsModule.Runtime.Client.Scripts;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.PetsSystem
{
    public class PlayersPetsViewControllerProviderService : IPlayersPetsViewControllerProviderService
    {
        public IReadOnlyDictionary<NetworkConnection, PlayerPetsViewController> PlayersViewControllers =>
            _playersFactory.ClientsComponents.ToDictionary(pair => pair.Key,
                pair => pair.Value.PetsViewController);

        private readonly PlayersFactory _playersFactory;

        public PlayersPetsViewControllerProviderService(PlayersFactory playersFactory)
        {
            _playersFactory = playersFactory;
        }
    }
}