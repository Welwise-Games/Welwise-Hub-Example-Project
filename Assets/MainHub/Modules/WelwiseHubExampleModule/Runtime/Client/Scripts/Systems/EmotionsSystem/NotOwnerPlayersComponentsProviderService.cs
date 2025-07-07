using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.NotOwner;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.EmotionsSystem
{
    public class NotOwnerPlayersComponentsProviderService : INotOwnerPlayersComponentsProviderService
    {
        public IReadOnlyDictionary<NetworkConnection, PlayerEmotionsComponents> EmotionsComponents =>
            _playersFactory?.ClientsComponents.ToDictionary(pair => pair.Key,
                pair => pair.Value.PlayerEmotionsComponents);

        private readonly PlayersFactory _playersFactory;

        public NotOwnerPlayersComponentsProviderService(PlayersFactory playersFactory)
        {
            _playersFactory = playersFactory;
        }
    }
}