using FishNet.Broadcast;
using UnityEngine;

namespace WelwiseHubBotsModule.Runtime.Shared.Scripts
{
    public struct PlayBotEmotionBroadcast : IBroadcast
    {
        public readonly string EmotionIndex;
        public readonly GameObject BotGameObject;

        public PlayBotEmotionBroadcast(string emotionIndex, GameObject botGameObject)
        {
            EmotionIndex = emotionIndex;
            BotGameObject = botGameObject;
        }
    }
}