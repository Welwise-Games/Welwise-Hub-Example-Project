using UnityEngine;
using WelwiseItemInShopModule.Client.Scripts;
using WelwisePetsModule.Runtime.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Client.Scripts.SetPet
{
    public class SetPetsPopupController : SetItemsPopupController<SelectedPetData, SelectedPetsData>
    {
        public SetPetsPopupController(PetsConfig itemsConfig,
            PetsViewConfig petsViewConfig,
            ISetItemsUIFactory<SelectedPetData, SelectedPetsData,
                SetItemButtonController<SelectedPetData, SelectedPetsData>> setItemsUiFactory,
            SetItemsPopup popup,
            Transform buttonsParent,
            SetItemsModel<SelectedPetData, SelectedPetsData> setItemsModel,
            float scaleMultiplierOnBecomeTarget, float speedChangingScaleOnSetTargetScale) :
            base(itemsConfig, petsViewConfig, setItemsUiFactory, popup,
                buttonsParent, setItemsModel, LocalizationTablesHolder.SetPetsPopup,
                LocalizationKeysHolder.MaximumIsNPets, scaleMultiplierOnBecomeTarget,
                speedChangingScaleOnSetTargetScale)
        {
        }
    }
}