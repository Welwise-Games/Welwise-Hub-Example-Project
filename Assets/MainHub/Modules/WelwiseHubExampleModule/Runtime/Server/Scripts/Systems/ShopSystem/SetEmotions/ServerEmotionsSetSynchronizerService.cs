using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem.SettingEmotions;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.ShopSystem.SetEmotions
{
    public class ServerEmotionsSetSynchronizerService
    {
        private readonly ClientsSelectedEmotionsDataProviderService _clientsSelectedEmotionsDataProviderService;

        public ServerEmotionsSetSynchronizerService(ServerManager serverManager, ClientsSelectedEmotionsDataProviderService clientsSelectedEmotionsDataProviderService)
        {
            _clientsSelectedEmotionsDataProviderService = clientsSelectedEmotionsDataProviderService;
            serverManager.RegisterBroadcast<SetSelectedEmotionsBroadcast>(
                HandleSetSelectedEmotions);
        }
        
        public void HandleSetSelectedEmotions(NetworkConnection networkConnection,
            SetSelectedEmotionsBroadcast selectedEmotionsBroadcast, Channel channel) =>
            _clientsSelectedEmotionsDataProviderService.TryUpdatingClientItemsSelectedData(networkConnection, selectedEmotionsBroadcast.Data);
    }
}