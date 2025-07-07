using System.IO;
using Cysharp.Threading.Tasks;
using FishNet.Managing.Client;
using UnityEngine;
using UnityEngine.Networking;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure
{
    public static class NetworkTools
    {
        public static async UniTask<bool> StartClientConnection(ClientManager clientManager) =>
            clientManager.StartConnection(
                (await LoadClientSettingsAsync()).Address, 7777);

        private static async UniTask<ClientNetworkSettingsData> LoadClientSettingsAsync()
        {
            ClientNetworkSettingsData settingsData;
            var url = Path.Combine(Application.streamingAssetsPath, "network_settings.json");

            using var request = UnityWebRequest.Get(url);
            try
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to load client settings: " + request.error);
                    settingsData = GetDefaultSettings();
                }
                else
                {
                    var json = request.downloadHandler.text;
                    settingsData = json.GetDeserializedWithoutNulls<ClientNetworkSettingsData>();
                    Debug.Log($"Loaded client settings: Address = {settingsData.Address}");
                }
            }
            catch
            {
                Debug.LogError("Failed to load client settings: " + request.error);
                settingsData = GetDefaultSettings();
            }

            return settingsData;
        }

        private static ClientNetworkSettingsData GetDefaultSettings() => new("localhost");
    }
}