using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing.Scened;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.HubSystem;
using WelwiseSharedModule.Runtime.Server.Scripts;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem
{
    public class Hub : IRoom
    {
        public IReadOnlyCollection<NetworkConnection> ConnectedClientsNetworkConnections => _connectedClientsNetworkConnections;
        
        public SceneLoadData SceneLoadData { get; }
        public int Id { get; }
        public int PeopleCount => _connectedClientsNetworkConnections.Count;

        public readonly SharedHubSerializableComponents Instance;
        
        private readonly HashSet<NetworkConnection> _connectedClientsNetworkConnections = new HashSet<NetworkConnection>();


        public Hub(int id, SceneLoadData sceneLoadData, SharedHubSerializableComponents instance)
        {
            SceneLoadData = sceneLoadData;
            Instance = instance;
            Id = id;
        }

        public void AddClient(NetworkConnection conn)
        {
            _connectedClientsNetworkConnections.Add(conn);
        }

        public void RemoveClient(NetworkConnection conn) => _connectedClientsNetworkConnections.Remove(conn);
    }
}