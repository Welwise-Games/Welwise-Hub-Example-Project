using System;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine.SceneManagement;

namespace WelwiseSharedModule.Runtime.Server.Scripts
{
    public class ServerSceneManagementService
    {
        public event Action<NetworkConnection, Scene> ClientLoadedScene;

        public void TryInvokingClientLoadedScene(ClientPresenceChangeEventArgs args)
        {
            if (args.Added)
                ClientLoadedScene?.Invoke(args.Connection, args.Scene);
        }
    }
}