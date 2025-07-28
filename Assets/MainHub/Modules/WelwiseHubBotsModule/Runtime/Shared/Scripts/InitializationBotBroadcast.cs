using FishNet.Broadcast;
using UnityEngine;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubBotsModule.Runtime.Shared.Scripts
{
    public struct InitializationBotBroadcast : IBroadcast
    {
        public readonly GameObject BotGameObject;
        public readonly string Nickname;
        public readonly string SerializedBotCustomizationData;
        public readonly string SerializedBotSelectedPetsData;

        public InitializationBotBroadcast(GameObject botGameObject, string nickname, CustomizationData customizationData, SelectedPetsData selectedPetsData)
        {
            BotGameObject = botGameObject;
            Nickname = nickname;
            SerializedBotCustomizationData = customizationData.GetJsonSerializedObjectWithoutNulls();
            SerializedBotSelectedPetsData = selectedPetsData.GetJsonSerializedObjectWithoutNulls();
        }
    }
}