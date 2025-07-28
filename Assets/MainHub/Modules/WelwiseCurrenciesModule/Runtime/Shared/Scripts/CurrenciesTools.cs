using System;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseCurrenciesModule.Runtime.Shared.Scripts
{
    public static class CurrenciesTools
    {
        public static void SubscribeAllCurrenciesTo(this ICurrenciesProviderService currenciesProviderService,
            Action<CurrencyType, Currency, int, SetCurrencyNumberReason> action)
            => currenciesProviderService.CurrencyByType.ForEach(pair =>
                pair.Value.NumberChanged += (number, reason) => action?.Invoke(pair.Key, pair.Value, number, reason));

        public static void SaveCurrencyInData(this CurrenciesData currenciesData,
            CurrencyType currencyType, Currency currency, int number, SetCurrencyNumberReason reason)
        {
            currenciesData.CurrenciesValues[currencyType] = number;
            Debug.Log($"Number of {currencyType} changed: {number}. Reason: {reason}");
        }
    }
}