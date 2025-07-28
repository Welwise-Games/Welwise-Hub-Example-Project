using System;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseItemInShopModule.Client.Scripts;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SetEmotions
{
    public class SetEmotionsPopupController : SetItemsPopupController<SelectedEmotionData, SelectedEmotionsData>
    {
        public SetEmotionsPopupController(EmotionsAnimationsConfig itemsConfig,
            EmotionsViewConfig emotionsViewConfig,
            ISetItemsUIFactory<SelectedEmotionData, SelectedEmotionsData,
                SetItemButtonController<SelectedEmotionData, SelectedEmotionsData>> setItemsUiFactory,
            SetItemsPopup popup,
            Transform buttonsParent,
            SetItemsModel<SelectedEmotionData, SelectedEmotionsData> setItemsModel,
            float scaleMultiplierOnBecomeTarget, float speedChangingScaleOnSetTargetScale) :
            base(itemsConfig, emotionsViewConfig, setItemsUiFactory, popup,
                buttonsParent, setItemsModel, LocalizationTablesHolder.SetEmotionsPopup,
                LocalizationKeysHolder.MaximumIsNEmotions, scaleMultiplierOnBecomeTarget,
                speedChangingScaleOnSetTargetScale)
        {
        }
    }
}