using FishNet.Broadcast;
using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.HubSystem.Network
{
    public struct HubInitializationDependencies : IBroadcast
    { 
        public readonly GameObject HubInstance;

        public HubInitializationDependencies(GameObject hubInstance)
        {
            HubInstance = hubInstance;
        }
    }
}