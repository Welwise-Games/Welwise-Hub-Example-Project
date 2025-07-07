using FishNet.Managing.Client;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem.SettingEmotions;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions.Network.Owner
{
    public class OwnerEmotionsSettingSynchronizerService
    {
        private readonly ClientManager _clientManager;

        public OwnerEmotionsSettingSynchronizerService(
            OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProvider, ClientManager clientManager,
            SettingEmotionsUIFactory settingEmotionsUIFactory)
        {
            _clientManager = clientManager;
            SubscribeToSendingBroadcastOnChangedEmotions(ownerSelectedEmotionsDataProvider, settingEmotionsUIFactory);
        }
        
        private void SubscribeToSendingBroadcastOnChangedEmotions(
            OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProvider,
            SettingEmotionsUIFactory settingEmotionsUIFactory)
        {
            settingEmotionsUIFactory.CreatedSettingEmotionsPopupController += controller =>
            {
                controller.SettingEmotionsModel.AppliedChanges += data =>
                {
                    if (ownerSelectedEmotionsDataProvider
                            .GetUpdatedSelectionEmotionsData(data.SelectedEmotions, shouldGetOnlyOne: true).Count <= 0) return;
                    
                    _clientManager.Broadcast(new SettingSelectedEmotionsDependencies(data.SelectedEmotions));
                };
            };
        }
    }
}