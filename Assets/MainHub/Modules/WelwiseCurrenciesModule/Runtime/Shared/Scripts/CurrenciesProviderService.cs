using System.Collections.Generic;
using System.Linq;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseCurrenciesModule.Runtime.Shared.Scripts
{
    public class CurrenciesProviderService : ICurrenciesProviderService
    {
        public IReadOnlyDictionary<CurrencyType, Currency> CurrencyByType => _currencyByType;
        private Dictionary<CurrencyType, Currency> _currencyByType = new Dictionary<CurrencyType, Currency>();

        public CurrenciesProviderService()
        {
            
        }

        public CurrenciesProviderService(CurrenciesData data) => TryInitializing(data);
        
        public void TryInitializing(CurrenciesData data)
        {
            if (data == null)
                return;
            
            _currencyByType = CollectionTools.ParseEnumToList<CurrencyType>()
                .Select(GetCurrency).ToDictionary(pair => pair.Key, pair => pair.Value);

            KeyValuePair<CurrencyType, Currency> GetCurrency(
                CurrencyType currencyType)
            {
                return KeyValuePair.Create(currencyType,
                    new Currency(data.CurrenciesValues.GetValueOrDefault(currencyType, 0)));
            }
            
            this.SubscribeAllCurrenciesTo(data.SaveCurrencyInData);
        }

        public void Dispose()
        {
            _currencyByType.ForEach(currency => currency.Value.Dispose());
            _currencyByType.Clear();
        }
    }
}