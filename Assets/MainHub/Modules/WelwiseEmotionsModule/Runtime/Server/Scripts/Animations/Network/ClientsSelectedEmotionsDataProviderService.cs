using System;
using System.Collections.Generic;
using FishNet.Connection;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network
{
    public class ClientsSelectedEmotionsDataProviderService
    {
        public IReadOnlyDictionary<NetworkConnection, ClientSelectedEmotionsData> ClientsData => _clientsData;

        private readonly Dictionary<NetworkConnection, ClientSelectedEmotionsData> _clientsData =
            new Dictionary<NetworkConnection, ClientSelectedEmotionsData>();

        public event Action<NetworkConnection, ClientSelectedEmotionsData> UpdatedData, AddedData;

        public void TryAddingClientSelectedEmotionsData(NetworkConnection networkConnection, ClientSelectedEmotionsData data)
        {
            if (data == null)
                return;
            
            _clientsData.Add(networkConnection, data);
            AddedData?.Invoke(networkConnection, data);
        }

        public void TryRemovingClientSelectedEmotionsData(NetworkConnection networkConnection) =>
            _clientsData.Remove(networkConnection);

        public void TryUpdatingClientSelectedEmotionsData(NetworkConnection networkConnection,
            List<SelectedEmotionData> data)
        {
            var updatedAnyData = false;

            foreach (var emotionData in data)
            {
                TryUpdatingClientSelectedEmotionData(networkConnection, emotionData.EmotionIndex, emotionData.IndexInsideCircle, out var successfully);
                updatedAnyData = updatedAnyData || successfully;
            }
            
            if (updatedAnyData)
                UpdatedData?.Invoke(networkConnection, _clientsData[networkConnection]);
        }

        private void TryUpdatingClientSelectedEmotionData(NetworkConnection networkConnection, string emotionIndex,
            int indexInsideCircle, out bool successfully)
        {
            var emotionsData = _clientsData.GetValueOrDefault(networkConnection);
            var data = emotionsData?.SelectedEmotions.SafeGet(indexInsideCircle);

            successfully = data != null;
            
            if (!successfully)
                return;

            data.EmotionIndex = emotionIndex;
        }
    }
}