using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseSharedModule.Runtime.Client.Scripts.Interfaces;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions
{
    public class SettingEmotionsModel : IModifiable
    {
        public bool IsModified =>
            _ownerSelectedEmotionsDataProviderService
                .GetUpdatedSelectionEmotionsData(
                    _lastClientSelectedEmotionsDataSnapshotOnApplyOrRevertChanges.SelectedEmotions,
                    TemporaryClientSelectedEmotionsData.SelectedEmotions,
                    shouldGetOnlyOne: true).Count > 0;

        public IReadOnlyList<SelectedEmotionData> TemporarySelectedEmotionsData =>
            TemporaryClientSelectedEmotionsData.SelectedEmotions;

        private ClientSelectedEmotionsData TemporaryClientSelectedEmotionsData { get; set; }

        private ClientSelectedEmotionsData _lastClientSelectedEmotionsDataSnapshotOnApplyOrRevertChanges;

        public event Action<ClientSelectedEmotionsData> AppliedChanges;

        private readonly OwnerSelectedEmotionsDataProviderService _ownerSelectedEmotionsDataProviderService;


        public SettingEmotionsModel(OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProviderService)
        {
            _ownerSelectedEmotionsDataProviderService = ownerSelectedEmotionsDataProviderService;

            RevertToSavedClientEmotionsData();

            ownerSelectedEmotionsDataProviderService.UpdatedEmotionsData += data => RevertToSavedClientEmotionsData();
        }

        public void ApplyChanges()
        {
            _lastClientSelectedEmotionsDataSnapshotOnApplyOrRevertChanges =
                GetClientSelectedEmotionsDataSnapshot(TemporarySelectedEmotionsData);
            
            AppliedChanges?.Invoke(TemporaryClientSelectedEmotionsData);
        }

        public void RevertToSavedClientEmotionsData()
        {
            TemporaryClientSelectedEmotionsData =
                GetClientSelectedEmotionsDataSnapshot(_ownerSelectedEmotionsDataProviderService
                    .GetAllSelectedEmotionsData());

            _lastClientSelectedEmotionsDataSnapshotOnApplyOrRevertChanges =
                GetClientSelectedEmotionsDataSnapshot(TemporarySelectedEmotionsData);
        }

        private ClientSelectedEmotionsData GetClientSelectedEmotionsDataSnapshot(
            IReadOnlyList<SelectedEmotionData> selectedEmotionData) =>
            new(selectedEmotionData
                .Select(data => new SelectedEmotionData(data.IndexInsideCircle, data.EmotionIndex)).ToList());

        public SelectedEmotionData GetFirstTemporarySelectedEmotionDataWithoutEmotion()
            => TemporarySelectedEmotionsData.FirstOrDefault(data => data.EmotionIndex == null);

        public void UpdateSelectedEmotionData(int indexInsideCircle, string emotionIndex, bool shouldAppointEmotionIndex)
        {
            TemporaryClientSelectedEmotionsData.SelectedEmotions[indexInsideCircle] =
                new SelectedEmotionData(indexInsideCircle, shouldAppointEmotionIndex ? emotionIndex : null);
        }
    }
}