using System;
using System.Collections.Generic;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public class OwnerSelectedItemsDataProviderService<TSelectedItemData, TClientSelectedItemsData>
        where TSelectedItemData : class, ISelectedItemData
        where TClientSelectedItemsData : IClientSelectedItemsData<TSelectedItemData>
    {
        public event Action<List<TSelectedItemData>> UpdatedItemData;

        private readonly List<TSelectedItemData> _selectedItemsData;

        public OwnerSelectedItemsDataProviderService(List<TSelectedItemData> selectedItemsData)
        {
            _selectedItemsData = selectedItemsData;
        }

        public TSelectedItemData GetSelectedDataByItemIndex(string index)
            => _selectedItemsData.Find(data => data.Index == index);

        public IReadOnlyList<TSelectedItemData> GetAllSelectedItemsData() =>
            _selectedItemsData;

        public TSelectedItemData GetItemDataByOrdinalIndex(int ordinalIndex) =>
            _selectedItemsData.SafeGet(ordinalIndex);

        public List<TSelectedItemData> GetUpdatedSelectedItemData(List<TSelectedItemData> currentSelectedData,
            List<TSelectedItemData> inputSelectedData,
            Action<TSelectedItemData, TSelectedItemData> added = null, bool shouldGetOnlyOne = false)
        {
            var updatedItemsData = new List<TSelectedItemData>();

            foreach (var inputItemData in inputSelectedData)
            {
                if (inputItemData == null)
                    continue;

                var currentItemData = currentSelectedData.SafeGet(inputItemData.OrdinalIndex);

                if (currentItemData == null || inputItemData.Index == currentItemData.Index)
                    continue;

                added?.Invoke(inputItemData, currentItemData);
                updatedItemsData.Add(currentItemData);

                if (shouldGetOnlyOne)
                    break;
            }

            return updatedItemsData;
        }

        public List<TSelectedItemData> GetUpdatedSelectedItemData(List<TSelectedItemData> inputItemData,
            Action<TSelectedItemData, TSelectedItemData> added = null, bool shouldGetOnlyOne = false)
            => GetUpdatedSelectedItemData(_selectedItemsData, inputItemData, added, shouldGetOnlyOne);


        public void TryUpdatingSelectedItemsData(TClientSelectedItemsData clientsSelectedITemsData)
        {
            var updatedItemData = GetUpdatedSelectedItemData(clientsSelectedITemsData.SelectedItemsData,
                (inputItemData, currentItemData) =>
                    currentItemData.Index = inputItemData.Index);

            if (updatedItemData.Count > 0)
                UpdatedItemData?.Invoke(updatedItemData);
        }
    }
}