using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure
{
    #if UNITY_WEBGL
    public static class AddressablesPathHolder
    {
        public static string RemoteLoadPath;
        
        [RuntimeInitializeOnLoadMethod]
        private static async void SetRuntimeLoadPath()
        {
            RemoteLoadPath = (await ClientFilesLoadingTools.LoadRemoteAddressablesSettingsData())
                .AddressablesRemoteLoadingURL;
        }
    }
    #endif
}