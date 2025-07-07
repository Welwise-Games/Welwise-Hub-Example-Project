using FishNet.Broadcast;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubBotsModule.Runtime.Shared.Scripts
{
    public struct BotChangedCustomizationDataBroadcast : IBroadcast
    {
        public readonly int BotObjectId;
        public readonly string NewSerializedData;

        public BotChangedCustomizationDataBroadcast(int botObjectId, CustomizationData customizationData)
        {
            BotObjectId = botObjectId;
            NewSerializedData = customizationData.GetJsonSerializedObjectWithoutNulls();
        } 
    }
}