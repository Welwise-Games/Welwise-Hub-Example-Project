using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
using WelwiseItemInShopModule.Client.Scripts;
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
                (emotionIndex, emotionOrdinalIndex) =>
                    ownerEmotionsPlayingSynchronizerService.SendPlayingEmotionAnimationBroadcast(
                        emotionOrdinalIndex);

        public static async UniTask InitializeAsync(
            DataContainer<EmotionsEntryPointData> emotionsEntryPointDataContainer,
            INotOwnerPlayersComponentsProviderService notOwnerPlayersComponentsProviderService,
            ClientManager clientManager,
            IPlayerData playerData,
            IAssetLoader assetLoader, EmotionsConfigProviderService emotionsConfigProviderService = null,
            EmotionsViewConfigProviderService emotionsViewConfigProviderService = null,
            EmotionsAnimationsConfig emotionsAnimationsConfig = null)
        {
            emotionsViewConfigProviderService ??= new EmotionsViewConfigProviderService(assetLoader);
            emotionsConfigProviderService ??= new EmotionsConfigProviderService(assetLoader);
            emotionsAnimationsConfig ??= await emotionsConfigProviderService.GetEmotionsAnimationsConfig();
            
            var ownerSelectedEmotionsProviderService =
                new OwnerSelectedEmotionsDataProviderService(Enumerable
                    .Range(0, emotionsAnimationsConfig.MaxSelectedItemsNumber)
                    .Select(i => new SelectedEmotionData(i)).ToList());

            var emotionsViewFactory = new EmotionsViewFactory(emotionsViewConfigProviderService);

            var emotionsCircleFactory =
                new EmotionsCircleFactory(ownerSelectedEmotionsProviderService, emotionsConfigProviderService,
                    emotionsViewFactory, emotionsViewConfigProviderService, assetLoader);

            var ownerEmotionsPlayingSynchronizer =
                new OwnerEmotionsPlayingSynchronizerService(ownerSelectedEmotionsProviderService, clientManager);

            new NotOwnerEmotionsSynchronizerService(notOwnerPlayersComponentsProviderService,
                emotionsAnimationsConfig, clientManager, emotionsViewFactory);

            SetItemsEntryPointTools.SubscribeToSaveMetaverseOnUpdate<SelectedEmotionData,
                SelectedEmotionsData, OwnerSelectedEmotionsDataProviderService>(
                ownerSelectedEmotionsProviderService, playerData, ()
                    => new SelectedEmotionsData(ownerSelectedEmotionsProviderService.GetAllSelectedItemsData()
                        .ToList(), emotionsAnimationsConfig), SelectedEmotionsDataFieldNameForMetaverseSavings);

            emotionsEntryPointDataContainer.Data = new EmotionsEntryPointData(ownerSelectedEmotionsProviderService,
                emotionsCircleFactory, ownerEmotionsPlayingSynchronizer,
                emotionsViewFactory);
        }
    }
}