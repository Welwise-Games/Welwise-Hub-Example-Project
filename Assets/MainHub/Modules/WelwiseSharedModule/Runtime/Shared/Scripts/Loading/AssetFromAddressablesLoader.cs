#if ADDRESSABLES
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;
using Object = UnityEngine.Object;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Loading
{
    public class AssetFromAddressablesLoader : IAssetLoader
    {
        public async UniTask<GameObject> GetInstantiatedGameObjectAsync(string assetId, Vector3? position = null,
            Quaternion? rotation = null,
            Transform parent = null)
        {
            var handle = position.HasValue || rotation.HasValue
                ? Addressables.InstantiateAsync(assetId, new InstantiationParameters(
                    position ?? Vector3.zero, rotation ?? Quaternion.identity, parent))
                : Addressables.InstantiateAsync(assetId, parent);

            var instance = await handle;

            if (!instance)
                throw new NullReferenceException($"Asset as addressable with assetId {assetId} not found");

            instance.GetOrAddComponent<DestroyObserver>().Destroyed
                += () => Addressables.ReleaseInstance(instance);
            
            return instance;
        }

        public async UniTask<T> GetLoadedAssetAsync<T>(string assetId) where T : Object => await Addressables.LoadAssetAsync<T>(assetId);

        public async UniTask<IEnumerable<T>> GetLoadedAssetsAsync<T>(string labelOrFolderPath) where T : Object
        {
            var locations = await Addressables.LoadResourceLocationsAsync(labelOrFolderPath, typeof(T));
            var tasks = locations.Select(location => Addressables.LoadAssetAsync<T>(location).Task.AsUniTask());

            return await UniTask.WhenAll(tasks);
        }
    }
}
#endif