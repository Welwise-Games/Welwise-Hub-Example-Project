using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Object = UnityEngine.Object;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public class SetItemsUIFactory<TSelectedItemData, TClientSelectedItemsData>
        where TSelectedItemData : class, ISelectedItemData
        where TClientSelectedItemsData : IClientSelectedItemsData<TSelectedItemData>
    {
        public event Action<SetItemsPopupController<TSelectedItemData, TClientSelectedItemsData>>
            CreatedSetItemsPopupController;

        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();


        private readonly string _setItemButtonAssetId;
        private readonly string _setItemsPopupAssetId;

        public SetItemsUIFactory(IAssetLoader assetLoader,
            string setItemsButtonAssetId, string setItemsPopupAssetId)
        {
            _assetLoader = assetLoader;
            _setItemButtonAssetId = setItemsButtonAssetId;
            _setItemsPopupAssetId = setItemsPopupAssetId;
        }

        public async UniTask DisposeAsync() => await _container.DestroyAndClearAllImplementationsAsync();

        public async UniTask<TPopupController> GetSetItemsPopupControllerAsync<TPopupController>(
                Transform popupTransform, Func<SetItemsPopup, UniTask<TPopupController>> getSetItemsPopupControllerFunc)
            where TPopupController : SetItemsPopupController<TSelectedItemData, TClientSelectedItemsData>
        {
            return await _container.GetControllerAsync<TPopupController, SetItemsPopup>(
                    _setItemsPopupAssetId, _assetLoader,
                    async popup =>
                    {
                        var popupController = await getSetItemsPopupControllerFunc.Invoke(popup);

                        _container.RegisterAndGetSingleByType(popupController);
                        CreatedSetItemsPopupController?.Invoke(popupController);

                        popup.transform.SetSiblingIndex(popupTransform.childCount - 2);
                    }, parent: popupTransform, shouldAppointParentAfterInstantiate: true);
        }

        public async UniTask<SetItemButtonView> GetSetItemButtonView(Transform parent)
        {
            var prefab =
                await _container.GetOrLoadAndRegisterObjectAsync<SetItemButtonView>(_setItemButtonAssetId,
                    _assetLoader,
                    shouldCreate: false);

            return Object.Instantiate(prefab, parent);
        }
    }
}