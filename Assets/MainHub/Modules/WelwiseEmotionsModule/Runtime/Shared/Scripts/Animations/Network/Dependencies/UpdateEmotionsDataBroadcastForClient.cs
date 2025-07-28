using FishNet.Broadcast;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network.Dependencies
{
    public struct UpdateEmotionsDataBroadcastForClient : IBroadcast
    {
        public readonly SelectedEmotionsData Data;

        public UpdateEmotionsDataBroadcastForClient(SelectedEmotionsData data)
        {
            Data = data;
        }
    }
}