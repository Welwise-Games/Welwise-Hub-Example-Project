using System;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure
{
    [Serializable]
    public struct RemoteAddressablesSettingsData
    {
        public readonly string AddressablesRemoteLoadingURL;

        public RemoteAddressablesSettingsData(string addressablesRemoteLoadingURL)
        {
            AddressablesRemoteLoadingURL = addressablesRemoteLoadingURL;
        }
    }
}