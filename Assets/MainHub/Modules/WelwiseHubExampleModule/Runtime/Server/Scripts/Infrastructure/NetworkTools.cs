using System;
using System.IO;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure
{
    public static class NetworkTools
    {
        public static ServerNetworkSettings LoadServerSettings()
        {
            var path = Path.Combine(Application.dataPath, "../network_settings.json");

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return json.GetDeserializedWithoutNulls<ServerNetworkSettings>();
            }

            return new ServerNetworkSettings(7777);
        }
    }

    [Serializable]
    public struct ServerNetworkSettings
    {
        public readonly int Port;

        public ServerNetworkSettings(int port)
        {
            Port = port;
        }
    }
}