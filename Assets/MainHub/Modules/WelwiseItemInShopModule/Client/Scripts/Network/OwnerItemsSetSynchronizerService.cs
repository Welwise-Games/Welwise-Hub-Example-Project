using System;
using System.Collections.Generic;
using FishNet.Broadcast;
using FishNet.Managing.Client;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwiseItemInShopModule.Client.Scripts.Network
{
    public class OwnerItemsSetSynchronizerService<TSelectedItemData, TClientSelectedItemsData, TBroadcast>
        where TSelectedItemData : class, ISelectedItemData
        where TClientSelectedItemsData : IClientSelectedItemsData<TSelectedItemData>
        where TBroadcast : struct, IBroadcast
    {
        private readonly ClientManager _clientManager;
        private readonly Func<List<TSelectedItemData>, TBroadcast> _getBroadcastFunc;

        public OwnerItemsSetSynchronizerService(
            OwnerSelectedItemsDataProviderService<TSelectedItemData, TClientSelectedItemsData>
                ownerSelectedItemsDataProvider, ClientManager clientManager,
            SetItemsUIFactory<TSelectedItemData, TClientSelectedItemsData> setItemsUIFactory,
            Func<List<TSelectedItemData>, TBroadcast> getBroadcastFunc)
        {
            _clientManager = clientManager;
            _getBroadcastFunc = getBroadcastFunc;
            SubscribeToSendingBroadcastOnChangedItems(ownerSelectedItemsDataProvider, setItemsUIFactory);
        }

        private void SubscribeToSendingBroadcastOnChangedItems(
            OwnerSelectedItemsDataProviderService<TSelectedItemData, TClientSelectedItemsData>
                ownerSelectedItemsDataProvider,
            SetItemsUIFactory<TSelectedItemData, TClientSelectedItemsData> setItemsUIFactory)
        {
            setItemsUIFactory.CreatedSetItemsPopupController += controller =>
            {
                controller.SetItemsModel.AppliedChanges += data =>
                {
                    if (ownerSelectedItemsDataProvider
                            .GetUpdatedSelectedItemData(data.SelectedItemsData, shouldGetOnlyOne: true).Count <=
                        0) return;
                    
                    _clientManager.Broadcast(_getBroadcastFunc.Invoke(data.SelectedItemsData));
                };
            };
        }
    }
}