using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet.Managing.Client;
using UnityEngine;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseItemInShopModule.Client.Scripts;
using WelwisePetsModule.Runtime.Client.Scripts.SetPet;
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    public class PetsEntryPointTools
    {
        public const string SelectedPetsDataFieldNameForMetaverseSavings = "SelectedPetsData";

        public static async UniTask InitializeAsync(DataContainer<PetsEntryPointData> petsEntryPointDataContainer,
            IPlayerData playerData,
            IAssetLoader assetLoader, ClientManager clientManager,
            IPlayersPetsViewControllerProviderService playersPetsViewControllerProviderService,
            PetsViewFactory petsViewFactory = null,
            PetsConfigProviderService petsConfigProviderService = null,
            PetsViewConfigProviderService petsViewConfigProviderService = null, PetsConfig petsConfig = null)
        {
            petsConfigProviderService ??= new PetsConfigProviderService(assetLoader);
            petsViewConfigProviderService ??= new PetsViewConfigProviderService(assetLoader);
            petsConfig ??= await petsConfigProviderService.GetPetsConfigAsync();
            
            var ownerSelectedPetsProviderService =
                new OwnerSelectedPetsDataProviderService(Enumerable.Range(0, petsConfig.MaxSelectedItemsNumber)
                    .Select(i => new SelectedPetData(i)).ToList());

            petsViewFactory ??= new PetsViewFactory(petsViewConfigProviderService, assetLoader);

            var playersPetsSynchronizer = new PlayersPetsSynchronizer(playersPetsViewControllerProviderService,
                ownerSelectedPetsProviderService);

            clientManager.RegisterBroadcast<UpdatePetsDataBroadcastForClient>(playersPetsSynchronizer
                .UpdatePlayersPets);

            SetItemsEntryPointTools.SubscribeToSaveMetaverseOnUpdate<SelectedPetData,
                SelectedPetsData, OwnerSelectedPetsDataProviderService>(
                ownerSelectedPetsProviderService, playerData, ()
                    => new SelectedPetsData(ownerSelectedPetsProviderService.GetAllSelectedItemsData()
                        .ToList(), petsConfig), SelectedPetsDataFieldNameForMetaverseSavings);

            petsEntryPointDataContainer.Data =
                new PetsEntryPointData(ownerSelectedPetsProviderService, petsViewFactory, new BotsPetsDataProviderService());
        }
    }
}