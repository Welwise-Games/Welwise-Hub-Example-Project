using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseCurrenciesModule.Runtime.Shared.Scripts;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseCurrenciesModule.Runtime.Client.Scripts
{
    public static class CurrenciesSDKTools
    {
        private static readonly Dictionary<CurrencyType, string> SavingFieldNameByCurrencyType =
            new Dictionary<CurrencyType, string>()
            {
                { CurrencyType.Hard, "WCoins_Hard" },
                { CurrencyType.Soft, "WCoins_Soft" }
            };

        public static void SaveCurrencies(this IPlayerData playerData, CurrenciesData currenciesData) =>
            currenciesData.CurrenciesValues.ForEach(currencyPair =>
                TrySavingCurrency(playerData, currencyPair.Key, currencyPair.Value));

        public static CurrenciesData GetFilledCurrenciesDataFromSavings(this IPlayerData playerData)
            => new CurrenciesData()
            {
                CurrenciesValues = CollectionTools.ParseEnumToList<CurrencyType>().ToDictionary(
                    currencyType => currencyType, currencyType => TryGettingCurrencyValue(playerData, currencyType))
            };

        public static void TrySavingCurrency(this IPlayerData playerData, CurrencyType currencyType, int value)
        {
            if (!SavingFieldNameByCurrencyType.TryGetValue(currencyType, out var currencySavingFieldName))
            {
                Debug.LogError("Field name is not appointed for this currency. Appoint it!");
                return;
            }

            playerData.MetaverseData.SetInt(currencySavingFieldName, value);
            playerData.Save();
        }

        public static int TryGettingCurrencyValue(this IPlayerData playerData, CurrencyType currencyType)
        {
            if (SavingFieldNameByCurrencyType.TryGetValue(currencyType, out var currencySavingFieldName))
                return playerData.MetaverseData.GetInt(currencySavingFieldName);

            Debug.LogError("Field name is not appointed for this currency. Appoint it!");
            return 0;
        }
    }
}