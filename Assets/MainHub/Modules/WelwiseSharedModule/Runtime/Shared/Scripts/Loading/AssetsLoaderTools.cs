namespace WelwiseSharedModule.Runtime.Shared.Scripts.Loading
{
    public static class AssetsLoaderTools
    {
        public static IAssetLoader GetAssetLoader()
        {
#if ADDRESSABLES
            return new AssetFromAddressablesLoader();
#endif
            return new AssetFromResourcesLoader();
        }
    }
}