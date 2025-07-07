using Cysharp.Threading.Tasks;
using FishNet.Connection;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.GameStateMachinePart
{
    public class ClientInitializationGameState : IGameState<ClientData>
    {
        private readonly EventBus _eventBus;
        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly ClientsConfigsProviderService _sharedConfigsProviderService;
        private readonly EmotionsConfigsProviderService _emotionsConfigsProviderService;

        public ClientInitializationGameState(EventBus eventBus, ClientsDataProviderService clientsDataProviderService,
             ClientsConfigsProviderService sharedConfigsProviderService,
            EmotionsConfigsProviderService emotionsConfigsProviderService)
        {
            _eventBus = eventBus;
            _clientsDataProviderService = clientsDataProviderService;
            _sharedConfigsProviderService = sharedConfigsProviderService;
            _emotionsConfigsProviderService = emotionsConfigsProviderService;
        }

        public async UniTask EnterAsync(NetworkConnection networkConnection, ClientData clientData)
        {
            if (_clientsDataProviderService.Data.ContainsKey(networkConnection))
                return;

            //var clientData = await _dataBaseService.GetClientDataAsync(clientId);

            // if (clientData == null)
            // {
            //var clientsConfig = await _sharedConfigsProviderService.GetClientsConfigAsync();

            // var clientEquippedItemsData = new ClientEquippedItemsData(
            //     CollectionTools.ToList<ItemCategory>()
            //         .Where(category => category is not ItemCategory.All and not ItemCategory.Color)
            //         .Select(category => new EquippedItemData(null, new Dictionary<int, float>(), category))
            //         .ToList());

            clientData = new ClientData(
                clientData.AccountData,
                clientData.SelectedEmotionsData ??
                new ClientSelectedEmotionsData(await _emotionsConfigsProviderService.GetEmotionsAnimationsConfig()),
                new CustomizationData(clientData.CustomizationData.AppearanceData ??
                                            new ModelAppearanceData(
                                                (await _sharedConfigsProviderService.GetClientsConfigAsync())
                                                .PlayerDefaultClothesColorValue,
                                                (await _sharedConfigsProviderService.GetClientsConfigAsync())
                                                .DefaultPlayerSkinColorValue),
                    clientData.CustomizationData.EquippedItemsData));

            //await _dataBaseService.RegisterClientAsync(clientData);
            // }
            // else
            // {
            //     var clientDataWithTheSameId = _clientsDataSavingService.ClientsDataForSending.FirstOrDefault(data =>
            //         data.Value.AccountData.Id ==
            //         clientId).Value;
            //
            //     if (clientDataWithTheSameId != null)
            //         clientData.TryMergingClientsData(clientDataWithTheSameId);
            // }

            _clientsDataProviderService.AddClientData(networkConnection, clientData);

            _eventBus.Fire(new EnterServerStateEvent(GameState.Hub, networkConnection));
        }

        public async UniTask ExitAsync(NetworkConnection networkConnection)
        {
        }
    }
}