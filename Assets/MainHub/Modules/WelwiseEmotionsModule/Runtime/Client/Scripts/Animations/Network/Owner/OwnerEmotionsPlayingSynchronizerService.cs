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
            clientManager.RegisterBroadcast<UpdateEmotionsDataDependencies>(UpdateEmotionsData);
        }
        public void SendPlayingEmotionAnimationBroadcast(int emotionIndexInsideCircle)
        {
            _clientManager.Broadcast(
                new PlayingEmotionAnimationDependenciesForServer(emotionIndexInsideCircle));
        }

        private void UpdateEmotionsData(UpdateEmotionsDataDependencies dependencies, Channel channel)
            => _ownerSelectedEmotionsDataProvider.TryUpdatingSelectedEmotionsData(dependencies.Data);
    }
}