using System;
using System.Collections.Generic;

namespace WelwiseChangingClothesModule.Runtime.Shared.Scripts
{
    public class BotsCustomizationDataProviderService
    {
        public IReadOnlyDictionary<int, CustomizationData> BotsCustomizationData => _botsCustomizationData;
        public event Action<int, CustomizationData> ChangedBotCustomizationData;

        private readonly Dictionary<int, CustomizationData> _botsCustomizationData =
            new Dictionary<int, CustomizationData>();

        public void AddBotCustomizationData(int botObjectId, CustomizationData customizationData)
            => _botsCustomizationData.TryAdd(botObjectId, customizationData);
            
        public void TrySettingBotCustomizationData(int botObjectId, CustomizationData inputCustomizationData)
        {
            if (!_botsCustomizationData.ContainsKey(botObjectId))
                return;
            
            _botsCustomizationData[botObjectId] = inputCustomizationData;

            ChangedBotCustomizationData?.Invoke(botObjectId, inputCustomizationData);
        }
        
        public void RemoveBotCustomizationData(int botObjectId) => _botsCustomizationData.Remove(botObjectId);
        public void Dispose() => _botsCustomizationData.Clear();
    }
}