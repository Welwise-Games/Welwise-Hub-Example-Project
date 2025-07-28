using System;
using System.Collections.Generic;

namespace WelwiseCurrenciesModule.Runtime.Shared.Scripts
{
    public interface ICurrenciesProviderService : IDisposable
    {
        IReadOnlyDictionary<CurrencyType, Currency> CurrencyByType { get; }
        void TryInitializing(CurrenciesData currenciesData);
    }
}