using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Client.Scripts.Example;
using WelwiseGamesSDK;
using WelwiseGamesSDK.Shared;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseLoadingClothesModule.Runtime.Client.Scripts.Example
{
    public class ClothesLoader : MonoBehaviour
    {
        [SerializeField] private ColorableClothesViewSerializableComponents colorableClothesViewSerializableComponents;

        public async void Start()
        {
            var sdk = WelwiseSDK.Construct().AsTransient();
            await sdk.InitializeAsync();

            var assetsLoader = AssetsLoaderTools.GetAssetLoader();

            new ClothesEntryPoint(new ItemsViewConfigsProviderService(assetsLoader), assetsLoader).OnCreatePlayerAsync(
                colorableClothesViewSerializableComponents,
                sdk.PlayerData.GetEquippedItemsDataFromMetaverse());

            Debug.Log("Clothes loaded");
        }
    }
}