using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseItemInShopModule.Server.Scripts
{
    public class ClientsSelectedItemsDataProviderService<TClientSelectedData, TSelectedData>
        where TClientSelectedData : IClientSelectedItemsData<TSelectedData>
        where TSelectedData : class, ISelectedItemData
    {
        public IReadOnlyDictionary<NetworkConnection, TClientSelectedData> ClientsData => _clientsData;

        private readonly Dictionary<NetworkConnection, TClientSelectedData> _clientsData =
            new Dictionary<NetworkConnection, TClientSelectedData>();

        public event Action<NetworkConnection, TClientSelectedData> UpdatedData, AddedData;

        private readonly IItemsConfig<IIndexableItemConfig> _itemsConfig;

        public ClientsSelectedItemsDataProviderService(IItemsConfig<IIndexableItemConfig> itemsConfig)
        {
            _itemsConfig = itemsConfig;
        }

        public void TryAddingClientSelectedData(NetworkConnection networkConnection, TClientSelectedData data)
        {
            if (data == null)
                return;

            _clientsData.Add(networkConnection, data);
            AddedData?.Invoke(networkConnection, data);
        }

        public void TryRemovingClientSelectedData(NetworkConnection networkConnection) =>
            _clientsData.Remove(networkConnection);

        public void TryUpdatingClientItemsSelectedData(NetworkConnection networkConnection,
            List<TSelectedData> data)
        {
            var updatedAnyData = false;

            foreach (var itemData in data)
            {
                TryUpdatingClientSelectedItemData(networkConnection, itemData.Index, itemData.OrdinalIndex,
                    out var successfully);
                updatedAnyData = updatedAnyData || successfully;
            }

            if (updatedAnyData)
                UpdatedData?.Invoke(networkConnection, _clientsData[networkConnection]);
        }

        private void TryUpdatingClientSelectedItemData(NetworkConnection networkConnection, string itemIndex,
            int itemOrdinalIndex, out bool successfully)
        {
            var itemsData = _clientsData.GetValueOrDefault(networkConnection);
            var data = itemsData?.SelectedItemsData.SafeGet(itemOrdinalIndex);

            successfully = data != null && (itemIndex == null || _itemsConfig.Configs.Any(config => config.Index == itemIndex));
            
            if (!successfully)
                return;

            data.Index = itemIndex;
        }
    }
}