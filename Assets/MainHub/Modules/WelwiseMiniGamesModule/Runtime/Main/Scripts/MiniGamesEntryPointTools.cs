using Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

public static class MiniGamesEntryPointTools
{
    public static void Initialize(out MiniGamesEntryPointData data, IAssetLoader assetLoader)
    {
        var miniGamesConfigProviderService = new MiniGamesConfigProviderService(assetLoader);
        var miniGamesFactory = new MiniGamesFactory(assetLoader);

        data = new MiniGamesEntryPointData(miniGamesConfigProviderService, miniGamesFactory);
    }
}