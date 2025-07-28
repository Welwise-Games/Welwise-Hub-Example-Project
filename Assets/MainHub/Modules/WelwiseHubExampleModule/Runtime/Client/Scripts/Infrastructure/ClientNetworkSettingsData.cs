using System;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure
{
    [Serializable]
    public struct ClientNetworkSettingsData
    {
        public readonly string Address;

        public ClientNetworkSettingsData(string address)
        {
            Address = address;
        }
    }
}