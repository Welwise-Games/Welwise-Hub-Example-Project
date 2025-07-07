using System;
using FishNet.Object;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ShopController
    {
        public event Action StartedPreview;

        public readonly SkinColorChangerController PlayerPreviewSkinColorChangerController;
        public readonly ColorableClothesViewController PreviewColorableClothesViewController;
        
        private readonly OwnerPlayerMovementController _ownerPlayerMovementController;

        public ShopController(ShopSerializableComponents shopSerializableComponents, ClientData clientData, ItemsViewConfig itemsViewConfig,
            ClothesFactory clothesFactory, OwnerPlayerMovementController ownerPlayerMovementController)
        {
            _ownerPlayerMovementController = ownerPlayerMovementController;

            PreviewColorableClothesViewController = new ColorableClothesViewController(
                clientData.CustomizationData.EquippedItemsData, itemsViewConfig,
                shopSerializableComponents.ColorableClothesViewSerializableComponents, clothesFactory);

            PlayerPreviewSkinColorChangerController = new SkinColorChangerController(shopSerializableComponents
                    .PlayerPreviewSkinColorChangerSerializableComponents,
                clientData.CustomizationData.AppearanceData);

            shopSerializableComponents.OpenShopColliderObserver.Entered += TryStartingPreview;
        }

        public void StopPreview()
        {
            CursorSwitcherTools.TryDisablingCursor();
            _ownerPlayerMovementController.IsEnabled = true;
        }

        private void TryStartingPreview(Collider collision)
        {
            if (!collision.TryGetComponent<NetworkObject>(out var networkObject) || !networkObject.IsOwner)
                return;

            CursorSwitcherTools.TryEnablingCursor();
            _ownerPlayerMovementController.IsEnabled = false;

            StartedPreview?.Invoke();
        }
    }
}