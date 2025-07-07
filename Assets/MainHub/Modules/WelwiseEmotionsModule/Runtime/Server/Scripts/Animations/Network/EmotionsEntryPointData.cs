namespace WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network
{
    public class EmotionsEntryPointData
    {
        public readonly ClientsSelectedEmotionsDataProviderService ClientsSelectedEmotionsDataProviderService;

        public EmotionsEntryPointData(ClientsSelectedEmotionsDataProviderService clientsSelectedEmotionsDataProviderService)
        {
            ClientsSelectedEmotionsDataProviderService = clientsSelectedEmotionsDataProviderService;
        }
    }
}