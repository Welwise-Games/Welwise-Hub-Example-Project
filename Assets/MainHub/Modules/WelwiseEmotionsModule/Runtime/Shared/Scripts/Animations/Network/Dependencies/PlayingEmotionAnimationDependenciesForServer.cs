using FishNet.Broadcast;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network.Dependencies
{
    public struct PlayingEmotionAnimationDependenciesForServer : IBroadcast
    {
        public readonly int EmotionIndexInsideCircle;

        public PlayingEmotionAnimationDependenciesForServer(int emotionIndexInsideCircle)
        {
            EmotionIndexInsideCircle = emotionIndexInsideCircle;
        }
    }
}