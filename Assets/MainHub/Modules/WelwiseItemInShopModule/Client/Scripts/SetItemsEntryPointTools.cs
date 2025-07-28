using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public static class SetItemsEntryPointTools
    {
        public static void SubscribeToSaveMetaverseOnUpdate<TSelectedItemData, TClientSelectedItemsData,
            TOwnerSelectedItemsDataProviderService>(
            TOwnerSelectedItemsDataProviderService ownerSelectedItemsDataProviderService,
            IPlayerData playerData, Func<TClientSelectedItemsData> getClientSelectedItemsData,
            string savingsFieldName)
            where TOwnerSelectedItemsDataProviderService :
            OwnerSelectedItemsDataProviderService<TSelectedItemData, TClientSelectedItemsData>
            where TSelectedItemData : class, ISelectedItemData
            where TClientSelectedItemsData : IClientSelectedItemsData<TSelectedItemData>
        {
            ownerSelectedItemsDataProviderService.UpdatedItemData += updatedData =>
                SetOwnersMetaverseStringData(playerData,
                    getClientSelectedItemsData.Invoke(), savingsFieldName);
        }

        private static void SetOwnersMetaverseStringData<TClientSelectedItemsData>(IPlayerData playerData,
            TClientSelectedItemsData clientSelectedItemsData, string fieldName)
        {
            var data = clientSelectedItemsData.GetJsonSerializedObjectWithoutNulls();

            if (data == playerData.MetaverseData.GetString(fieldName))
                return;

            playerData.MetaverseData.SetString(fieldName, data);

            playerData.Save();
        }
    }
}