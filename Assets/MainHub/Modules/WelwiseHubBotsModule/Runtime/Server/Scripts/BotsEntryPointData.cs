namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotsEntryPointData
    {
        public readonly BotsFactory BotsFactory;
        public readonly BotsConfigsProviderService BotsConfigsProviderService;
        
        public BotsEntryPointData(BotsFactory botsFactory, BotsConfigsProviderService botsConfigsProviderService)
        {
            BotsFactory = botsFactory;
            BotsConfigsProviderService = botsConfigsProviderService;
        }
    }
}