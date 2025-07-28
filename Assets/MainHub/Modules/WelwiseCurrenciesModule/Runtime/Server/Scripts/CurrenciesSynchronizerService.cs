using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Transporting;
using WelwiseCurrenciesModule.Runtime.Shared.Scripts;

namespace WelwiseCurrenciesModule.Runtime.Server.Scripts
{
    public class CurrenciesSynchronizerService
    {
        private readonly ClientsCurrenciesProviderService _clientsCurrenciesProviderService;

        public CurrenciesSynchronizerService(ClientsCurrenciesProviderService clientsCurrenciesProviderService)
        {
            _clientsCurrenciesProviderService = clientsCurrenciesProviderService;
        }

        public void UpdateCurrency(NetworkConnection networkConnection,
            CurrencyNumberChangedBroadcastForServer broadcast, Channel channel)
        {
            _clientsCurrenciesProviderService.Services.GetValueOrDefault(networkConnection)
                ?.CurrencyByType[broadcast.CurrencyType].TrySettingNumber(broadcast.NewNumber, broadcast.Reason);
        }
    }
}