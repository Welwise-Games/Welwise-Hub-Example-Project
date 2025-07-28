using System.Collections.Generic;
using FishNet.Connection;
using WelwiseCurrenciesModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseCurrenciesModule.Runtime.Server.Scripts
{
    public class ClientsCurrenciesProviderService
    {
        public IReadOnlyDictionary<NetworkConnection, ICurrenciesProviderService> Services => _services;

        private readonly Dictionary<NetworkConnection, ICurrenciesProviderService> _services =
            new Dictionary<NetworkConnection, ICurrenciesProviderService>();

        public void Add(NetworkConnection networkConnection, CurrenciesData currenciesData)
            => _services.AddOrAppoint(networkConnection, new CurrenciesProviderService(currenciesData));

        public void TryRemoving(NetworkConnection networkConnection)
        {
            if (_services.Remove(networkConnection, out var service))
                service.Dispose();
        }
    }
}