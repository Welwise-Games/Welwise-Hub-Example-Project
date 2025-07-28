using FishNet.Broadcast;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network.Dependencies
{
    public struct PlayingEmotionAnimationDependenciesForServer : IBroadcast
    {
        public readonly int EmotionOrdinalIndex;

        public PlayingEmotionAnimationDependenciesForServer(int emotionOrdinalIndex)
        {
            EmotionOrdinalIndex = emotionOrdinalIndex;
        }
    }
}