using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    public class ClothesFactory
    {
        private readonly IAssetLoader _assetLoader;

        public ClothesFactory(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<ColorableClothesSerializableComponents> GetClothesInstanceAsync(
            ItemViewConfig itemConfig, Transform parent)
        {
            var prefab =

#if ADDRESSABLES
                await AssetProvider.LoadAsync<ColorableClothesSerializableComponents>(
                await itemConfig.PrefabReference.GetAssetIdAsync(), _assetLoader);
#else
                itemConfig.Prefab;
#endif

            var instance = Object.Instantiate(prefab, parent);
            SetPrefabInstanceLayer(instance.gameObject, parent.gameObject.layer);
            return instance;
        }

        private void SetPrefabInstanceLayer(GameObject instance, int targetLayer)
        {
            instance.GetComponentsInChildren<Transform>().ToList()
                .ForEach(transform => transform.gameObject.layer = targetLayer);
        }
    }
}