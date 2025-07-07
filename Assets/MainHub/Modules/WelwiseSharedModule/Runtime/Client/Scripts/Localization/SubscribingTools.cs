using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseSharedModule.Runtime.Client.Scripts.Localization
{
    public static class SubscribingTools
    {
        public static void SubscribeToLocalizationAndSubscribeOnDestroy(this GameObject gameObject,
            Action<Locale> actions)
        {
            if (!gameObject)
                return;

            LocalizationSettings.SelectedLocaleChanged += actions;
            
            gameObject.GetOrAddComponent<DestroyObserver>().Destroyed += () => LocalizationSettings.SelectedLocaleChanged -= actions;
        }
    }
}