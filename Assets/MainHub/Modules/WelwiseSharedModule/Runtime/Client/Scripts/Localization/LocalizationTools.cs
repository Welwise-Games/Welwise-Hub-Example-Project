using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;

namespace WelwiseSharedModule.Runtime.Client.Scripts.Localization
{
    public static class LocalizationTools
    {
        public static async UniTask<string> GetLocalizedStringAsync(string tableName, string key)
            => await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, key).Task;

        public static async UniTask<string> GetLocalizedStringAsync(string tableName, string key, string n1 = null, string n2 = null)
        {
            var localizedString = await GetLocalizedStringAsync(tableName, key);

            if (n1 != null)
                localizedString = localizedString.Replace(LocalizationKeysHolder.FirstVariableName, n1);

            if (n2 != null)
                localizedString = localizedString.Replace(LocalizationKeysHolder.SecondVariableName, n2);

            return localizedString;
        }
    }
}