using System;
using FishNet.Connection;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.Services.Data;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.Services
{
    public class ClientsCustomizationDataProviderService
    {
        public event Action<NetworkConnection, CustomizationData> ChangedClientPlayerCustomizationData
        {
            add => _sharedClientsCustomizationDataProviderService.ChangedClientPlayerCustomizationData += value;
            remove => _sharedClientsCustomizationDataProviderService.ChangedClientPlayerCustomizationData -= value;
        }

        private readonly SharedClientsCustomizationDataProviderService _sharedClientsCustomizationDataProviderService;

        public ClientsCustomizationDataProviderService(
            SharedClientsCustomizationDataProviderService sharedClientsCustomizationDataProviderService)
        {
            _sharedClientsCustomizationDataProviderService = sharedClientsCustomizationDataProviderService;
        }

        public void TrySettingClientCustomizationData(NetworkConnection networkConnection, CustomizationData customizationData) 
            => _sharedClientsCustomizationDataProviderService.TrySettingClientPlayerCustomizationData(networkConnection, customizationData);
    }
}