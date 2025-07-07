using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Managing.Object;
using FishNet.Object;
using GameKit.Dependencies.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts
{
    public static class NetworkAssetsLoader
    {
        private static readonly ushort LabelId = NetworkObjects.GetStableHashU16();
        private const string NetworkObjects = "NetworkObjects";

        public static async UniTask AddNetworkObjectsToSpawnableObjects()
        {
#if !ADDRESSABLES
            await AddResourcesNetworkObjectsToSpawnableObjects();
#else
            await AddAddressablesNetworkObjectsToSpawnableObjects();
#endif
        }

#if !ADDRESSABLES
        public static async UniTask AddResourcesNetworkObjectsToSpawnableObjects()
        {
            var allResources = Resources.LoadAll("", typeof(GameObject));

            var networkObjects = allResources.OfType<GameObject>()
                .Select(gameObject => gameObject.GetComponent<NetworkObject>()).Where(component => component != null).ToList();
            var spawnablePrefabs =
                (SinglePrefabObjects)InstanceFinder.NetworkManager.GetPrefabObjects<SinglePrefabObjects>(LabelId, true);

            spawnablePrefabs.AddObjects(networkObjects);
        }
#else
        public static async UniTask AddAddressablesNetworkObjectsToSpawnableObjects()
        {
            var spawnablePrefabs =
                (SinglePrefabObjects)InstanceFinder.NetworkManager.GetPrefabObjects<SinglePrefabObjects>(LabelId, true);

            var cache = CollectionCaches<NetworkObject>.RetrieveList();

            await Addressables.LoadAssetsAsync<GameObject>(NetworkObjects,
                gameObject => cache.Add(gameObject.GetComponent<NetworkObject>()));

            spawnablePrefabs.AddObjects(cache);
            CollectionCaches<NetworkObject>.Store(cache);
        }
#endif
    }
}