using System;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseCurrenciesModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwisePetsModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data
{
    [Serializable]
    public class ClientData
    {
        public ClientAccountData AccountData { get; set; }
        public SelectedEmotionsData SelectedEmotionsData { get; set; }
        public CustomizationData CustomizationData { get; set; }
        public CurrenciesData CurrenciesData { get; set; }
        public SelectedPetsData SelectedPetsData { get; set; }
        
        public ClientData()
        {
        }
        
        public ClientData(ClientAccountData accountData, SelectedEmotionsData selectedEmotionsData, 
            CustomizationData customizationData, CurrenciesData currenciesData, SelectedPetsData selectedPetsData)
        {
            AccountData = accountData;
            SelectedEmotionsData = selectedEmotionsData;
            CustomizationData = customizationData;
            CurrenciesData = currenciesData;
            SelectedPetsData = selectedPetsData;
        }
    }
}