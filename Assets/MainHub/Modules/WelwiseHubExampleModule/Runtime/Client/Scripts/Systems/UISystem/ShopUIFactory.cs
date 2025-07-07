using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseChangingNicknameModule.Runtime.Client.Scripts.Services;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.UI;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Object = UnityEngine.Object;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem
{
    public class ShopUIFactory
    {
        public event Action<ShopPopupController> CreatedShopPopupController;

        private readonly ItemsConfigsProviderService _itemsConfigsProviderService;
        private readonly ItemsViewConfigsProviderService _itemsViewConfigsProviderService;
        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly ClientsNicknamesProviderService _clientsNicknamesProviderService;
        private readonly SharedNicknamesConfigsProviderService _sharedNicknamesConfigsProviderService;
        private readonly UIFactory _uiFactory;
        private readonly IAssetLoader _assetLoader;

        private readonly Container _container = new Container();

        private const string ShopPopupAssetId =
#if ADDRESSABLES
        "ShopPopup";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/ShopPopup";
#endif

        private const string SelectionItemButton =
#if ADDRESSABLES
        "SelectionItemButton";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/SelectionItemButton";
#endif

        public ShopUIFactory(ClientsDataProviderService clientsDataProviderService,
            ClientsNicknamesProviderService clientsNicknamesProviderService,
            SharedNicknamesConfigsProviderService sharedNicknamesConfigsProviderService,
            ItemsConfigsProviderService itemsConfigsProviderService, ItemsViewConfigsProviderService itemsViewConfigsProviderService, UIFactory uiFactory, IAssetLoader assetLoader)
        {
            _clientsDataProviderService = clientsDataProviderService;
            _clientsNicknamesProviderService = clientsNicknamesProviderService;
            _sharedNicknamesConfigsProviderService = sharedNicknamesConfigsProviderService;
            _itemsConfigsProviderService = itemsConfigsProviderService;
            _uiFactory = uiFactory;
            _assetLoader = assetLoader;
            _itemsViewConfigsProviderService = itemsViewConfigsProviderService;
        }

        public async UniTask DisposeUIAsync()
            => await _container.DestroyAndClearAllImplementationsAsync();

        public async UniTask<SelectionItemButtonController> GetSelectionItemButtonControllerAsync(Transform parent,
            IIndexableItemConfig indexableItemConfig, SelectionItemButtonTargetStateAnimationConfig animationConfig)
        {
            var buttonPrefab = await _container.GetOrLoadAndRegisterObjectAsync<SelectionItemButtonView>(
                SelectionItemButton, _assetLoader, shouldCreate: false);
            var buttonInstance = Object.Instantiate(buttonPrefab, parent);

            return new SelectionItemButtonController(indexableItemConfig, buttonInstance, animationConfig);
        }

        public ShopPopupController GetShopPopupController() =>
            _container.GetSingleByType<ShopPopupController>();

        public async UniTask<ShopPopupController> GetCreatedShopPopupControllerAsync(ShopController shopController,
            ShopSettingEquippedItemsModel shopSettingEquippedItemsModel)
        {
            return await _container.GetControllerAsync<ShopPopupController, ShopPopup>(ShopPopupAssetId,
                _assetLoader, async popup =>
                {
                    var shopPopupController = new ShopPopupController(shopSettingEquippedItemsModel,
                        popup, await _itemsConfigsProviderService.GetItemsConfigAsync(), this,
                        _clientsDataProviderService,
                        _clientsNicknamesProviderService,
                        shopController.PlayerPreviewSkinColorChangerController,
                        shopController.PreviewColorableClothesViewController,
                        await _sharedNicknamesConfigsProviderService.GetSharedClientsNicknamesConfigAsync(), await _itemsViewConfigsProviderService.GetItemsViewConfigAsync());

                    _container.RegisterAndGetSingleByType(shopPopupController);

                    CreatedShopPopupController?.Invoke(shopPopupController);
                }, parent: (await _uiFactory.GetUIRootAsync()).SerializableComponents.transform);
        }
    }
}