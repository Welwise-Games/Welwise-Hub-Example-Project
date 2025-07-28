using FishNet.Broadcast;

namespace WelwiseCurrenciesModule.Runtime.Shared.Scripts
{
    public struct CurrencyNumberChangedBroadcastForServer : IBroadcast
    {
        public readonly int NewNumber;
        public readonly CurrencyType CurrencyType;
        public readonly SetCurrencyNumberReason Reason;

        public CurrencyNumberChangedBroadcastForServer(CurrencyType currencyType, int newNumber, SetCurrencyNumberReason reason)
        {
            CurrencyType = currencyType;
            NewNumber = newNumber;
            Reason = reason;
        }
    }
}