using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts
{
    [Serializable]
    public class ClientSelectedEmotionsData
    {
        public List<SelectedEmotionData> SelectedEmotions { get; set; }

        public ClientSelectedEmotionsData(List<SelectedEmotionData> selectedEmotions)
        {
            SelectedEmotions = selectedEmotions;
        }

        public ClientSelectedEmotionsData(EmotionsAnimationsConfig emotionsAnimationsConfig)
        {
            SelectedEmotions = Enumerable.Range(0, emotionsAnimationsConfig.MaxSelectedAnimationsNumber).Select(i =>
                new SelectedEmotionData(i, emotionsAnimationsConfig.EmotionsAnimationConfigs.SafeGet(i)?.EmotionIndex)).ToList();
        }

        public ClientSelectedEmotionsData()
        {
            
        }
    }
}