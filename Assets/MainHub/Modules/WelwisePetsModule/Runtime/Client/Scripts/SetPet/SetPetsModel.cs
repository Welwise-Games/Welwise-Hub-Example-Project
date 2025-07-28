using System;
using System.Collections.Generic;
using WelwiseItemInShopModule.Client.Scripts;
using WelwisePetsModule.Runtime.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Client.Scripts.SetPet
{
    public class SetPetsModel : SetItemsModel<SelectedPetData, SelectedPetsData>
    {
        public SetPetsModel(OwnerSelectedPetsDataProviderService
                ownerSelectedItemsDataProviderService,
            Func<IReadOnlyList<SelectedPetData>, SelectedPetsData> getClientSelectedPetsData,
            Func<int, string, SelectedPetData> getSelectedItemDataFunc) : base(
            ownerSelectedItemsDataProviderService, getClientSelectedPetsData, getSelectedItemDataFunc)
        {
        }
    }
}