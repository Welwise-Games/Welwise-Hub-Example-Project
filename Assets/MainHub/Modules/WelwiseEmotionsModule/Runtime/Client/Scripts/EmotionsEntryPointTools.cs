using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Client;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.NotOwner;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Circle;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    public static class EmotionsEntryPointTools
    {
        public const string SelectedEmotionsDataFieldNameForMetaverseSavings = "SelectedEmotionsData";

        public static void SubscribeAnimatorController(this PlayerEmotionsComponents playerEmotionsComponents,
            OwnerEmotionsPlayingSynchronizerService ownerEmotionsPlayingSynchronizerService) =>
            playerEmotionsComponents.EmotionsAnimatorController.StartedEmotionAnimation +=
                (emotionIndex, emotionIndexInsideCircle) =>
                    ownerEmotionsPlayingSynchronizerService.SendPlayingEmotionAnimationBroadcast(
                        emotionIndexInsideCircle);

        public static void Initialize(
            INotOwnerPlayersComponentsProviderService notOwnerPlayersComponentsProviderService,
            ClientManager clientManager,
            EmotionsAnimationsConfig emotionsAnimationsConfig,
            EmotionsConfigsProviderService emotionsConfigsProviderService,
            EmotionsViewConfigsProviderService emotionsViewConfigsProviderService, IPlayerData playerData,
            out EmotionsEntryPointData emotionsEntryPointData, IAssetLoader assetLoader)
        {
            var ownerSelectedEmotionsProviderService =
                new OwnerSelectedEmotionsDataProviderService(emotionsAnimationsConfig);

            var emotionsViewFactory = new EmotionsViewFactory(emotionsViewConfigsProviderService);

            var emotionsCircleFactory =
                new EmotionsCircleFactory(ownerSelectedEmotionsProviderService, emotionsConfigsProviderService,
                    emotionsViewFactory, emotionsViewConfigsProviderService, assetLoader);

            var ownerEmotionsPlayingSynchronizer =
                new OwnerEmotionsPlayingSynchronizerService(ownerSelectedEmotionsProviderService, clientManager);

            new NotOwnerEmotionsSynchronizerService(notOwnerPlayersComponentsProviderService,
                emotionsAnimationsConfig, clientManager, emotionsViewFactory);

            ownerSelectedEmotionsProviderService.UpdatedEmotionsData += updatedData =>
                SetOwnersMetaverseStringData(playerData,
                    new ClientSelectedEmotionsData(ownerSelectedEmotionsProviderService.GetAllSelectedEmotionsData()
                        .ToList()));

            emotionsEntryPointData = new EmotionsEntryPointData(ownerSelectedEmotionsProviderService,
                emotionsCircleFactory, ownerEmotionsPlayingSynchronizer,
                emotionsViewFactory);
        }

        private static void SetOwnersMetaverseStringData(IPlayerData playerData,
            ClientSelectedEmotionsData clientSelectedEmotionsData)
        {
            var data = clientSelectedEmotionsData.GetJsonSerializedObjectWithoutNulls();
            
            if (data == playerData.MetaverseData.GetString(SelectedEmotionsDataFieldNameForMetaverseSavings))
                return;
            
            playerData.MetaverseData.SetString(SelectedEmotionsDataFieldNameForMetaverseSavings, data);
            
            playerData.Save();
        }
    }
}