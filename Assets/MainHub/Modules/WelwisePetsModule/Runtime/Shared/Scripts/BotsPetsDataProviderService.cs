using System;
using System.Collections.Generic;

namespace WelwisePetsModule.Runtime.Shared.Scripts
{
    public class BotsPetsDataProviderService
    {
        public IReadOnlyDictionary<int, SelectedPetsData> PetsData => _petsData;
        public event Action<int, SelectedPetsData> ChangedBotPetsData;
        
        private readonly Dictionary<int, SelectedPetsData> _petsData = new Dictionary<int, SelectedPetsData>();

        public void AddBotData(int botObjectId, SelectedPetsData data)
            => _petsData.TryAdd(botObjectId, data);
        
        public void TrySettingBotData(int botObjectId, SelectedPetsData selectedPetsData)
        {
            if (!_petsData.ContainsKey(botObjectId))
                return;
            
            _petsData[botObjectId] = selectedPetsData;
            ChangedBotPetsData?.Invoke(botObjectId, selectedPetsData);
        }

        public void RemoveBotNickname(int botObjectId) => _petsData.Remove(botObjectId);
        public void Dispose() => _petsData.Clear();
    }
}