using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.ResourceLocations;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure
{
    public static class ClientFilesLoadingTools
    {
        public static async UniTask<RemoteAddressablesSettingsData> LoadRemoteAddressablesSettingsData() =>
            await TryLoadingRemoteJsonFile("config.json",
                () => new RemoteAddressablesSettingsData(
                    "https://welwisegames.ru/games/online/1/client/ServerData/WebGL/"));
        
        public static async UniTask<ClientNetworkSettingsData> LoadClientSettingsAsync() =>
            await TryLoadingRemoteJsonFile("network_settings.json", GetDefaultSettings);

        private static async UniTask<T> TryLoadingRemoteJsonFile<T>(string fileName, Func<T> defaultValueFunc)
        {
            T value;

            var url = Path.Combine(Application.streamingAssetsPath, fileName);
            
            using var request = UnityWebRequest.Get(url);
            try
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load {fileName}: " + request.error);
                    value = defaultValueFunc.Invoke();
                }
                else
                {
                    var json = request.downloadHandler.text;
                    value = json.GetFromJsonDeserializedWithoutNulls<T>();
                    Debug.Log($"Loaded {fileName}. URL: {url}, downloaded value: {json}");
                }
            }
            catch
            {
                Debug.LogError($"Failed to load {fileName}: " + request.error);
                value = defaultValueFunc.Invoke();
            }

            return value;
        }

        private static ClientNetworkSettingsData GetDefaultSettings() => new("localhost");
    }
}