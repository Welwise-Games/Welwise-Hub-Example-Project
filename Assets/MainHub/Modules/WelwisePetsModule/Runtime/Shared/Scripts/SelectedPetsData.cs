using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwisePetsModule.Runtime.Shared.Scripts
{
    [Serializable]
    public class SelectedPetsData : IClientSelectedItemsData<SelectedPetData>
    {
        public List<SelectedPetData> SelectedItemsData { get; set; }

        public SelectedPetsData(List<SelectedPetData> selectedItemsData, PetsConfig petsConfig)
        {
            SelectedItemsData = Enumerable.Range(0, petsConfig.MaxSelectedItemsNumber).Select(i =>
            {
                var selectedItemData = selectedItemsData.SafeGet(i);
                return new SelectedPetData(i, selectedItemData?.Index);
            }).ToList();
        }

        public SelectedPetsData(PetsConfig petsConfig)
        {
            SelectedItemsData = Enumerable.Range(0, petsConfig.MaxSelectedItemsNumber).Select(i =>
                new SelectedPetData(i)).ToList();
        }

        public SelectedPetsData()
        {
        }
    }
}