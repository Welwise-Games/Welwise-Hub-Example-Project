using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem.SettingEmotions;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.ShopSystem.SettingEmotions
{
    public class ServerEmotionsSettingSynchronizerService
    {
        private readonly ClientsSelectedEmotionsDataProviderService _clientsSelectedEmotionsDataProviderService;

        public ServerEmotionsSettingSynchronizerService(ServerManager serverManager, ClientsSelectedEmotionsDataProviderService clientsSelectedEmotionsDataProviderService)
        {
            _clientsSelectedEmotionsDataProviderService = clientsSelectedEmotionsDataProviderService;
            serverManager.RegisterBroadcast<SettingSelectedEmotionsDependencies>(
                HandleSettingSelectedEmotions);
        }
        
        public void HandleSettingSelectedEmotions(NetworkConnection networkConnection,
            SettingSelectedEmotionsDependencies selectedEmotionsDependencies, Channel channel) =>
            _clientsSelectedEmotionsDataProviderService.TryUpdatingClientSelectedEmotionsData(networkConnection, selectedEmotionsDependencies.Data);
    }
}