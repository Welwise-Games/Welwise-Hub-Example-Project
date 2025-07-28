using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseItemInShopModule.Client.Scripts;
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwisePetsModule.Runtime.Client.Scripts.SetPet
{
    public class SetPetsUIFactory : ISetItemsUIFactory<SelectedPetData, SelectedPetsData,
        SetItemButtonController<SelectedPetData, SelectedPetsData>>
    {
        public event Action<SetPetsPopupController> CreatedSetPetsPopupController;

        private readonly PetsConfigProviderService _petsConfigProviderService;
        private readonly PetsViewConfigProviderService _petsViewConfigProviderService;
        private readonly OwnerSelectedPetsDataProviderService _ownerSelectedPetsDataProviderService;

        public readonly SetItemsUIFactory<SelectedPetData, SelectedPetsData> SetItemsUIFactory;

        private const string SetPetButtonAssetId =
#if ADDRESSABLES
        "SetPetButton";
#else
            "WelwisePetsModule/Runtime/Client/Loadable/SetPetButton";
#endif

        private const string SetPetsPopupAssetId =
#if ADDRESSABLES
        "SetPetsPopup";
#else
            "WelwisePetsModule/Runtime/Client/Loadable/SetPetsPopup";
#endif


        public SetPetsUIFactory(PetsConfigProviderService petsConfigProviderService,
            PetsViewConfigProviderService petsViewConfigProviderService,
            OwnerSelectedPetsDataProviderService ownerSelectedPetsDataProviderService,
            IAssetLoader assetLoader)
        {
            _petsConfigProviderService = petsConfigProviderService;
            _ownerSelectedPetsDataProviderService = ownerSelectedPetsDataProviderService;
            SetItemsUIFactory =
                new SetItemsUIFactory<SelectedPetData, SelectedPetsData>(assetLoader,
                    SetPetButtonAssetId,
                    SetPetsPopupAssetId);
            _petsViewConfigProviderService = petsViewConfigProviderService;
        }

        public async UniTask<SetItemButtonController<SelectedPetData, SelectedPetsData>> GetNewSetItemButtonController(
                Transform parent, IItemViewConfig targetPetViewConfig, float scaleMultiplierOnBecomeTarget,
                float speedChangingScaleOnSetTargetState)
        {
            return new SetItemButtonController<SelectedPetData, SelectedPetsData>(
                await SetItemsUIFactory.GetSetItemButtonView(parent),
                targetPetViewConfig as PetViewConfig,
                _ownerSelectedPetsDataProviderService,
                scaleMultiplierOnBecomeTarget, speedChangingScaleOnSetTargetState, LocalizationTablesHolder.Pets);
        }

        public async UniTask<SetPetsPopupController> GetSetPetsPopupControllerAsync(Transform popupTransform,
            Transform buttonsParent,
            float scaleMultiplierOnBecomeTarget, float speedChangingScaleOnSetTargetState)
        {
            return await SetItemsUIFactory
                .GetSetItemsPopupControllerAsync(popupTransform,
                    async popup =>
                    {
                        var petsConfig = await _petsConfigProviderService.GetPetsConfigAsync();
                        var popupController = new SetPetsPopupController(
                            petsConfig,
                            await _petsViewConfigProviderService.GetPetsViewConfigAsync(), this, popup,
                            buttonsParent, new SetPetsModel(
                                _ownerSelectedPetsDataProviderService, (data) => new SelectedPetsData(
                                    data.ToList(), petsConfig),
                                (ordinalIndex, index) => new SelectedPetData(ordinalIndex, index)),
                            scaleMultiplierOnBecomeTarget, speedChangingScaleOnSetTargetState);

                        CreatedSetPetsPopupController?.Invoke(popupController);
                        return popupController;
                    });
        }
    }
}