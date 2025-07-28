using System;
using System.Collections.Generic;

namespace WelwiseCurrenciesModule.Runtime.Shared.Scripts
{
    [Serializable]
    public class CurrenciesData
    {
        public Dictionary<CurrencyType, int> CurrenciesValues { get; set; } = new Dictionary<CurrencyType, int>();

        public CurrenciesData()
        {
        }
    }
}