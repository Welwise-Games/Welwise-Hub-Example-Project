using UnityEngine;
using WelwiseItemInShopModule.Server.Scripts;
using WelwisePetsModule.Runtime.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Server.Scripts
{
    public class ClientsSelectedPetsDataProviderService : ClientsSelectedItemsDataProviderService<
        SelectedPetsData, SelectedPetData>
    {
        public ClientsSelectedPetsDataProviderService(PetsConfig itemsConfig) : base(itemsConfig)
        {
        }
    }
}