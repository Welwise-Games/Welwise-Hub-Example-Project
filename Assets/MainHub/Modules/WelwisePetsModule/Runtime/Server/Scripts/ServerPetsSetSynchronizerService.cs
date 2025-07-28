using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Server.Scripts;

namespace WelwisePetsModule.Runtime.Server.Scripts
{
    public class ServerPetsSetSynchronizerService
    {
        private readonly ServerManager _serverManager;
        private readonly ClientsSelectedPetsDataProviderService _clientsSelectedEmotionsDataProviderService;
        private readonly IVisibleClientsProviderService _visibleClientsProviderService;

        public ServerPetsSetSynchronizerService(ServerManager serverManager,
            ClientsSelectedPetsDataProviderService clientsSelectedEmotionsDataProviderService,
            IVisibleClientsProviderService visibleClientsProviderService)
        {
            _serverManager = serverManager;
            _clientsSelectedEmotionsDataProviderService = clientsSelectedEmotionsDataProviderService;
            _visibleClientsProviderService = visibleClientsProviderService;

            serverManager.RegisterBroadcast<SetSelectedPetsBroadcastForServer>(
                HandleSetSelectedPets);

            _clientsSelectedEmotionsDataProviderService.UpdatedData += SendUpdateSelectedPetsDataBroadcast;
            _clientsSelectedEmotionsDataProviderService.AddedData += SendUpdateSelectedPetsDataBroadcast;
        }

        private void SendUpdateSelectedPetsDataBroadcast(NetworkConnection networkConnection,
            SelectedPetsData data)
        {
            _serverManager.Broadcast(
                _visibleClientsProviderService.GetVisibleClientsForClient(networkConnection).Append(networkConnection).ToHashSet(),
                new UpdatePetsDataBroadcastForClient(data, networkConnection));
        }

        private void HandleSetSelectedPets(NetworkConnection networkConnection,
            SetSelectedPetsBroadcastForServer selectedPetBroadcast, Channel channel)
        {
            _clientsSelectedEmotionsDataProviderService.TryUpdatingClientItemsSelectedData(networkConnection,
                selectedPetBroadcast.Data);
        }
    }
}