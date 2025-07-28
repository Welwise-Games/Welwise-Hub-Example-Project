using FishNet.Managing.Client;
using FishNet.Transporting;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network.Dependencies;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner
{
    public class OwnerEmotionsPlayingSynchronizerService
    {
        private readonly OwnerSelectedEmotionsDataProviderService _ownerSelectedEmotionsDataProvider;
        private readonly ClientManager _clientManager;

        public OwnerEmotionsPlayingSynchronizerService(
            OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProvider, ClientManager clientManager)
        {
            _ownerSelectedEmotionsDataProvider = ownerSelectedEmotionsDataProvider;
            _clientManager = clientManager;
            clientManager.RegisterBroadcast<UpdateEmotionsDataBroadcastForClient>(UpdateEmotionsData);
        }
        public void SendPlayingEmotionAnimationBroadcast(int emotionOrdinalIndex)
        {
            _clientManager.Broadcast(
                new PlayingEmotionAnimationDependenciesForServer(emotionOrdinalIndex));
        }

        private void UpdateEmotionsData(UpdateEmotionsDataBroadcastForClient broadcastForClient, Channel channel)
            => _ownerSelectedEmotionsDataProvider.TryUpdatingSelectedItemsData(broadcastForClient.Data);
    }
}