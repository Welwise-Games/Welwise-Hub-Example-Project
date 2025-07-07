using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Loading
{
    public class AssetFromResourcesLoader : IAssetLoader
    {
        private const float MaxLoadingTime = 5;

        public async UniTask<GameObject> GetInstantiatedGameObjectAsync(string assetId, Vector3? position = null,
            Quaternion? rotation = null,
            Transform parent = null)
        {
            var asset = await Resources.LoadAsync(assetId);

            if (asset == null)
                throw new NullReferenceException($"Asset as resources with path {assetId} not found");

            var instance = position.HasValue || rotation.HasValue
                ? UnityEngine.Object.Instantiate(asset,
                    position ?? Vector3.zero,
                    rotation ?? Quaternion.identity, parent) as GameObject
                : Object.Instantiate(asset, parent) as GameObject;

            return instance;
        }

        public async UniTask<T> GetLoadedAssetAsync<T>(string assetId) where T : Object
        {
            var request = Resources.LoadAsync(assetId);

            var timer = new Timer();

            timer.Ended += () => throw new Exception($"Asset is not loaded under id {assetId}");
            timer.TryStartingCountingTime(MaxLoadingTime);

            request.completed += _ => timer.TryStoppingCountingTime();

            return await request as T;
        }

        public async UniTask<IEnumerable<T>> GetLoadedAssetsAsync<T>(string labelOrFolderPath) where T : Object
        {
            return Resources.LoadAll<T>(labelOrFolderPath);
        }
    }
}