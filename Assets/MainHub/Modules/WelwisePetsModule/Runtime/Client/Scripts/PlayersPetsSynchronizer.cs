using FishNet.Transporting;
using UnityEngine;
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using UniTaskExtensions = Cysharp.Threading.Tasks.UniTaskExtensions;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    public class PlayersPetsSynchronizer
    {
        private readonly IPlayersPetsViewControllerProviderService _playersPetsViewControllerProviderService;
        private readonly OwnerSelectedPetsDataProviderService _ownerSelectedPetsDataProviderService;

        public PlayersPetsSynchronizer(
            IPlayersPetsViewControllerProviderService playersPetsViewControllerProviderService,
            OwnerSelectedPetsDataProviderService ownerSelectedPetsDataProviderService)
        {
            _playersPetsViewControllerProviderService = playersPetsViewControllerProviderService;
            _ownerSelectedPetsDataProviderService = ownerSelectedPetsDataProviderService;
        }

        public void UpdatePlayersPets(UpdatePetsDataBroadcastForClient broadcast, Channel channel)
        {
            if (broadcast.NetworkConnection.IsOwners())
                _ownerSelectedPetsDataProviderService.TryUpdatingSelectedItemsData(broadcast.Data);
            

            if (_playersPetsViewControllerProviderService.PlayersViewControllers.TryGetValue(
                    broadcast.NetworkConnection, out var playerPetsViewController))
                UniTaskExtensions.Forget(playerPetsViewController.UpdatePetsViewAsync(broadcast.Data));   
        }
    }
}