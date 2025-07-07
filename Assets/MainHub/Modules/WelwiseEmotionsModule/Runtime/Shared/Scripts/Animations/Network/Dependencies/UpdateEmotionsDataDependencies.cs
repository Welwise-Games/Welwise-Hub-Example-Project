using FishNet.Broadcast;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network.Dependencies
{
    public struct UpdateEmotionsDataDependencies : IBroadcast
    {
        public readonly ClientSelectedEmotionsData Data;

        public UpdateEmotionsDataDependencies(ClientSelectedEmotionsData data)
        {
            Data = data;
        }
    }
}