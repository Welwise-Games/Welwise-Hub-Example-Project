using FishNet.Broadcast;
using FishNet.Connection;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network.Dependencies
{
    public struct PlayingEmotionAnimationDependenciesForClient : IBroadcast
    {
        public readonly string EmotionIndex;
        public readonly NetworkConnection PlayingPlayerNetworkConnection;

        public PlayingEmotionAnimationDependenciesForClient(string emotionIndex, NetworkConnection playingPlayerNetworkConnection)
        {
            EmotionIndex = emotionIndex;
            PlayingPlayerNetworkConnection = playingPlayerNetworkConnection;
        }
    }
}