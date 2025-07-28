using FishNet.Managing.Client;
using WelwiseCurrenciesModule.Runtime.Shared.Scripts;
using WelwiseGamesSDK.Shared.Modules;

namespace WelwiseCurrenciesModule.Runtime.Client.Scripts
{
    public static class CurrenciesTools
    {
        public static void TryInitializingAndSubscribe(this ICurrenciesProviderService currenciesProviderService, CurrenciesData currenciesData, IPlayerData playerData,
            ClientManager clientManager)
        {
            currenciesProviderService.TryInitializing(currenciesData);
            currenciesProviderService.SubscribeAllCurrenciesTo((currencyType, currency, number, reason) =>
            {
                playerData.TrySavingCurrency(currencyType, number);
                    
                if (reason is not SetCurrencyNumberReason.FromServerReward and not SetCurrencyNumberReason.Initialization)
                    clientManager.Broadcast(new CurrencyNumberChangedBroadcastForServer(currencyType, number, reason));
            });
        }
    }
}