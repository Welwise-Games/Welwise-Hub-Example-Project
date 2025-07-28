using FishNet.Managing.Server;
using WelwiseCurrenciesModule.Runtime.Shared.Scripts;

namespace WelwiseCurrenciesModule.Runtime.Server.Scripts
{
    public static class CurrenciesTools
    {
        public static void Subscribe(this CurrenciesSynchronizerService currenciesSynchronizerService, ServerManager serverManager)
        {
            serverManager.RegisterBroadcast<CurrencyNumberChangedBroadcastForServer>(currenciesSynchronizerService.UpdateCurrency);
        }
    }
}