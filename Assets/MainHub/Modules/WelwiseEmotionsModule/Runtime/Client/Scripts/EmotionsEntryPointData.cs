using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Circle;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    public class EmotionsEntryPointData
    {
        public readonly OwnerSelectedEmotionsDataProviderService OwnerSelectedEmotionsDataProviderService;
        public readonly EmotionsCircleFactory EmotionsCircleFactory;
        public readonly OwnerEmotionsPlayingSynchronizerService OwnerEmotionsPlayingSynchronizerService;
        public readonly EmotionsViewFactory EmotionsViewFactory;

        public EmotionsEntryPointData(OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProviderService,
            EmotionsCircleFactory emotionsCircleFactory, OwnerEmotionsPlayingSynchronizerService ownerEmotionsPlayingSynchronizerService,
            EmotionsViewFactory emotionsViewFactory)
        {
            OwnerSelectedEmotionsDataProviderService = ownerSelectedEmotionsDataProviderService;
            EmotionsCircleFactory = emotionsCircleFactory;
            OwnerEmotionsPlayingSynchronizerService = ownerEmotionsPlayingSynchronizerService;
            EmotionsViewFactory = emotionsViewFactory;
        }
    }
}