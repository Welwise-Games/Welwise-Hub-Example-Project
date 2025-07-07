using System.Collections.Generic;
using FishNet.Connection;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.NotOwner
{
    public interface INotOwnerPlayersComponentsProviderService
    {
        IReadOnlyDictionary<NetworkConnection, PlayerEmotionsComponents> EmotionsComponents { get; }
    }
}