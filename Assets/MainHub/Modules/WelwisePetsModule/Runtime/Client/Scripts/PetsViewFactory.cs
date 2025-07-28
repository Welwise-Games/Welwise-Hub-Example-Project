using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwisePetsModule.Runtime.Client.Scripts.SetPet;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    public class PetsViewFactory
    {
        private readonly PetsViewConfigProviderService _petsViewConfigProviderService;
        private readonly IAssetLoader _assetLoader;

        public PetsViewFactory(PetsViewConfigProviderService petsViewConfigProviderService, IAssetLoader assetLoader)
        {
            _petsViewConfigProviderService = petsViewConfigProviderService;
            _assetLoader = assetLoader;
        }

        public async UniTask<PetViewController> GetCreatedPetAndViewControllerAsync(string index, int ordinalIndex,
            Transform petOwnerTransform)
        {
            var petsViewConfig = await _petsViewConfigProviderService.GetPetsViewConfigAsync();

            var petViewConfig = petsViewConfig.TryGettingPetViewConfigByIndex(index);

            var prefab =
#if ADDRESSABLES
                
                await AssetProvider.LoadAsync<GameObject>(await petViewConfig.PrefabReference.GetAssetIdAsync(), _assetLoader);
#else
                petViewConfig.Prefab;
#endif

            var offset = petsViewConfig.PetOffsetFromPetOwnerByOrdinalIndex.SafeGet(ordinalIndex);
            
            var instance = Object.Instantiate(prefab, petOwnerTransform.position + petOwnerTransform.TransformDirection(offset), Quaternion.identity);

            petOwnerTransform.gameObject.GetOrAddComponent<DestroyObserver>().Destroyed +=
                () => Object.Destroy(instance);

            return petViewConfig == null
                ? null
                : new PetViewController(instance.transform, petOwnerTransform,
                    offset, petViewConfig);
        }
    }
}