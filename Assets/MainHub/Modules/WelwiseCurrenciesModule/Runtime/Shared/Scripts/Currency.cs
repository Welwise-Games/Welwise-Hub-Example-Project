using System;

namespace WelwiseCurrenciesModule.Runtime.Shared.Scripts
{
    public class Currency
    {
        public int Number => _number;

        public event Action<int, SetCurrencyNumberReason> NumberChanged;
        
        private int _number;

        public Currency(int number) => TrySettingNumber(number, SetCurrencyNumberReason.Initialization);

        public void TrySettingNumber(int newNumber, SetCurrencyNumberReason reason)
        {
            if (newNumber < 0)
                return;

            _number = newNumber;
            NumberChanged?.Invoke(newNumber, reason);
        }

        public void Dispose() => NumberChanged = null;
    }
}