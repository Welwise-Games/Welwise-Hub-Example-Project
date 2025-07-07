using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner
{
    public class OwnerSelectedEmotionsDataProviderService
    {
        public event Action<List<SelectedEmotionData>> UpdatedEmotionsData;

        private readonly List<SelectedEmotionData> _selectedEmotionsData;

        public OwnerSelectedEmotionsDataProviderService(EmotionsAnimationsConfig emotionsAnimationsConfig)
        {
            _selectedEmotionsData = Enumerable.Range(0, emotionsAnimationsConfig.MaxSelectedAnimationsNumber)
                .Select(i => new SelectedEmotionData(i)).ToList();
        }

        public SelectedEmotionData GetEmotionDataByEmotionIndex(string emotionIndex)
            => _selectedEmotionsData.Find(data => data.EmotionIndex == emotionIndex);

        public IReadOnlyList<SelectedEmotionData> GetAllSelectedEmotionsData() =>
            _selectedEmotionsData;

        public SelectedEmotionData GetEmotionDataByCircleIndex(int indexInsideCircle) =>
            _selectedEmotionsData.SafeGet(indexInsideCircle);

        public List<SelectedEmotionData> GetUpdatedSelectionEmotionsData(List<SelectedEmotionData> currentEmotionsData, List<SelectedEmotionData> inputEmotionsData,
            Action<SelectedEmotionData, SelectedEmotionData> added = null, bool shouldGetOnlyOne = false)
        {
            var updatedEmotionsData = new List<SelectedEmotionData>();
            
            foreach (var inputEmotionData in inputEmotionsData)
            {
                if (inputEmotionData == null)
                    continue;

                var currentEmotionData = currentEmotionsData.SafeGet(inputEmotionData.IndexInsideCircle);

                if (currentEmotionData == null || inputEmotionData.EmotionIndex == currentEmotionData.EmotionIndex)
                    continue;
                
                added?.Invoke(inputEmotionData, currentEmotionData);
                updatedEmotionsData.Add(currentEmotionData);
                
                if (shouldGetOnlyOne)
                    break;
            }

            return updatedEmotionsData;
        }

        public List<SelectedEmotionData> GetUpdatedSelectionEmotionsData(List<SelectedEmotionData> inputEmotionsData,
            Action<SelectedEmotionData, SelectedEmotionData> added = null, bool shouldGetOnlyOne = false)
            => GetUpdatedSelectionEmotionsData(_selectedEmotionsData, inputEmotionsData, added, shouldGetOnlyOne);


        public void TryUpdatingSelectedEmotionsData(ClientSelectedEmotionsData clientsSelectedEmotionsData)
        {
            var updatedEmotionsData = GetUpdatedSelectionEmotionsData(clientsSelectedEmotionsData.SelectedEmotions,
                (inputEmotionData, currentEmotionData) =>
                    currentEmotionData.EmotionIndex = inputEmotionData.EmotionIndex);
            
            if (updatedEmotionsData.Count > 0)
                UpdatedEmotionsData?.Invoke(updatedEmotionsData);
        }
    }
}