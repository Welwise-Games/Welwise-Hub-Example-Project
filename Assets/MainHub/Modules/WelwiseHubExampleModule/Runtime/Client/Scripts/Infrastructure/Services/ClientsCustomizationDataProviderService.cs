using System;
using FishNet.Connection;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.Services
{
    public class ClientsCustomizationDataProviderService
    {
        public event Action<NetworkConnection, CustomizationData> ChangedClientCustomizationData
        {
            add => _sharedClientsCustomizationDataProviderService.ChangedClientPlayerCustomizationData += value;
            remove => _sharedClientsCustomizationDataProviderService.ChangedClientPlayerCustomizationData -= value;
        }

        private readonly SharedClientsCustomizationDataProviderService _sharedClientsCustomizationDataProviderService;
        private readonly ClientsDataProviderService _clientsDataProviderService;

        public ClientsCustomizationDataProviderService(
            SharedClientsCustomizationDataProviderService sharedClientsCustomizationDataProviderService,
            ClientsDataProviderService clientsDataProviderService)
        {
            _sharedClientsCustomizationDataProviderService = sharedClientsCustomizationDataProviderService;
            _clientsDataProviderService = clientsDataProviderService;
        }
        public void TrySettingClientCustomizationData(NetworkConnection networkConnection,
            CustomizationData inputCustomizationData)
        {
            _sharedClientsCustomizationDataProviderService.TrySettingClientPlayerCustomizationData(networkConnection,
                inputCustomizationData, false);
        }

        public CustomizationData GetClientPlayerCustomizationData(NetworkConnection networkConnection) =>
            _sharedClientsCustomizationDataProviderService.GetClientPlayerCustomizationData(networkConnection);

        public bool AreCustomizationDataEqual(CustomizationData firstCustomizationData,
            CustomizationData secondCustomizationData) =>
            _sharedClientsCustomizationDataProviderService.AreCustomizationDataEqual(firstCustomizationData,
                secondCustomizationData);

        public bool CanSetClientPlayerCustomizationData(NetworkConnection networkConnection,
            CustomizationData customizationData) =>
            _sharedClientsCustomizationDataProviderService.CanSetClientPlayerCustomizationData(networkConnection,
                customizationData, out var data);
    }
}