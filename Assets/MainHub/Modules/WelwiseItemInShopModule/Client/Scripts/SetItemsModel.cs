using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Interfaces;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public class SetItemsModel<TSelectedItemData, TClientSelectedItemsData> : IModifiable
        where TSelectedItemData : class, ISelectedItemData
        where TClientSelectedItemsData : IClientSelectedItemsData<TSelectedItemData>
    {
        public bool IsModified =>
            _ownerSelectedItemsDataProviderService
                .GetUpdatedSelectedItemData(
                    _lastClientSelectedItemsDataSnapshotOnApplyOrRevertChanges.SelectedItemsData,
                    TemporaryClientSelectedItemsData.SelectedItemsData,
                    shouldGetOnlyOne: true).Count > 0;

        public IReadOnlyList<TSelectedItemData> TemporarySelectedItemsData =>
            TemporaryClientSelectedItemsData.SelectedItemsData;

        private TClientSelectedItemsData TemporaryClientSelectedItemsData { get; set; }

        public event Action<TClientSelectedItemsData> AppliedChanges, UpdatedTemporaryData;

        private TClientSelectedItemsData _lastClientSelectedItemsDataSnapshotOnApplyOrRevertChanges;

        private readonly OwnerSelectedItemsDataProviderService<TSelectedItemData, TClientSelectedItemsData>
            _ownerSelectedItemsDataProviderService;

        private readonly Func<IReadOnlyList<TSelectedItemData>, TClientSelectedItemsData> _getClientSelectedItemsData;
        private readonly Func<int, string, TSelectedItemData> _getSelectedItemDataFunc;


        public SetItemsModel(
            OwnerSelectedItemsDataProviderService<TSelectedItemData, TClientSelectedItemsData>
                ownerSelectedItemsDataProviderService,
            Func<IReadOnlyList<TSelectedItemData>, TClientSelectedItemsData> getClientSelectedItemsData,
            Func<int, string, TSelectedItemData> getSelectedItemDataFunc)
        {
            _ownerSelectedItemsDataProviderService = ownerSelectedItemsDataProviderService;
            _getClientSelectedItemsData = getClientSelectedItemsData;
            _getSelectedItemDataFunc = getSelectedItemDataFunc;

            RevertToSavedClientItemsData();

            ownerSelectedItemsDataProviderService.UpdatedItemData +=
                data => RevertToSavedClientItemsData();
        }

        public void ApplyChanges()
        {
            _lastClientSelectedItemsDataSnapshotOnApplyOrRevertChanges =
                GetClientSelectedItemsDataSnapshot(TemporarySelectedItemsData);

            AppliedChanges?.Invoke(TemporaryClientSelectedItemsData);
        }

        public void RevertToSavedClientItemsData()
        {
            TemporaryClientSelectedItemsData =
                GetClientSelectedItemsDataSnapshot(_ownerSelectedItemsDataProviderService
                    .GetAllSelectedItemsData());

            _lastClientSelectedItemsDataSnapshotOnApplyOrRevertChanges =
                GetClientSelectedItemsDataSnapshot(TemporarySelectedItemsData);
            
            UpdatedTemporaryData?.Invoke(TemporaryClientSelectedItemsData);
        }


        private TClientSelectedItemsData GetClientSelectedItemsDataSnapshot(
            IReadOnlyList<TSelectedItemData> selectedItemsData) =>
            _getClientSelectedItemsData.Invoke(selectedItemsData);

        public TSelectedItemData GetFirstTemporarySelectedItemDataWithoutItem()
            => TemporarySelectedItemsData.FirstOrDefault(data => data.Index == null);

        public void UpdateSelectedItemData(int itemOrdinalIndex, string itemIndex, bool shouldAppointItemIndex)
        {
            TemporaryClientSelectedItemsData.SelectedItemsData[itemOrdinalIndex] =
                _getSelectedItemDataFunc.Invoke(itemOrdinalIndex, shouldAppointItemIndex ? itemIndex : null);

            UpdatedTemporaryData?.Invoke(TemporaryClientSelectedItemsData);
        }
    }
}