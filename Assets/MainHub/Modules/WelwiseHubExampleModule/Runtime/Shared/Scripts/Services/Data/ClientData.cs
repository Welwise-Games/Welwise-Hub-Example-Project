using System;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data
{
    [Serializable]
    public class ClientData
    {
        public ClientAccountData AccountData { get; set; }
        public ClientSelectedEmotionsData SelectedEmotionsData { get; set; }
        public CustomizationData CustomizationData { get; set; }
        
        public ClientData()
        {
        }
        
        public ClientData(ClientAccountData accountData, ClientSelectedEmotionsData selectedEmotionsData, 
            CustomizationData customizationData)
        {
            AccountData = accountData;
            SelectedEmotionsData = selectedEmotionsData;
            CustomizationData = customizationData;
        }
    }
}