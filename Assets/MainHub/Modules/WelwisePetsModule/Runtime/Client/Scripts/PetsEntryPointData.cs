using WelwisePetsModule.Runtime.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    public class PetsEntryPointData
    {
        public readonly OwnerSelectedPetsDataProviderService OwnerSelectedPetsDataProviderService;
        public readonly PetsViewFactory PetsViewFactory;
        public readonly BotsPetsDataProviderService BotsPetsDataProviderService;

        public PetsEntryPointData(OwnerSelectedPetsDataProviderService ownerSelectedPetsDataProviderService, PetsViewFactory petsViewFactory, BotsPetsDataProviderService botsPetsDataProviderService)
        {
            OwnerSelectedPetsDataProviderService = ownerSelectedPetsDataProviderService;
            PetsViewFactory = petsViewFactory;
            BotsPetsDataProviderService = botsPetsDataProviderService;
        }
    }
}