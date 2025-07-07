using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Loading
{
    public interface IAssetLoader
    {
        UniTask<GameObject> GetInstantiatedGameObjectAsync(string assetId, Vector3? position = null,
            Quaternion? rotation = null, Transform parent = null);

        UniTask<T> GetLoadedAssetAsync<T>(string assetId) where T : Object;

        UniTask<IEnumerable<T>> GetLoadedAssetsAsync<T>(string labelOrFolderPath) where T : Object;
    }
}