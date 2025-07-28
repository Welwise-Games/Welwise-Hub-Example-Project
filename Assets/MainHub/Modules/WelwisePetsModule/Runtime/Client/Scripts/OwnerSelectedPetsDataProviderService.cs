using System.Collections.Generic;
using WelwiseItemInShopModule.Client.Scripts;
using WelwisePetsModule.Runtime.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    public class OwnerSelectedPetsDataProviderService : OwnerSelectedItemsDataProviderService<SelectedPetData, SelectedPetsData>
    {
        public OwnerSelectedPetsDataProviderService(
            List<SelectedPetData> selectedPetsData) : base(selectedPetsData)
        {
            
        }
    }
}