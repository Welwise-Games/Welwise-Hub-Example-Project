namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    public class MiniGamesEntryPointData
    {
        public readonly MiniGamesConfigProviderService MiniGamesConfigProviderService;
        public readonly MiniGamesFactory MiniGamesFactory;

        public MiniGamesEntryPointData(MiniGamesConfigProviderService miniGamesConfigProviderService, MiniGamesFactory miniGamesFactory)
        {
            MiniGamesConfigProviderService = miniGamesConfigProviderService;
            MiniGamesFactory = miniGamesFactory;
        }
    }
}