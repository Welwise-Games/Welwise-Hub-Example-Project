using System;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network
{
    [Serializable]
    public class SelectedEmotionData
    {
        public string EmotionIndex { get; set; }
        public int IndexInsideCircle { get; set; }

        public SelectedEmotionData(int indexInsideCircle, string emotionIndex = null)
        {
            EmotionIndex = emotionIndex;
            IndexInsideCircle = indexInsideCircle;
        }
        
        public SelectedEmotionData() {}
    }
}