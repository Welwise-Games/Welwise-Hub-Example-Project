using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Transporting;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubBotsModule.Runtime.Client.Scripts
{
    public class BotsClientSynchronizer
    {
        private readonly BotsFactory _botsFactory;
        private readonly EmotionsConfigsProviderService _emotionsConfigsProviderService;
        private readonly EmotionsViewFactory _emotionsViewFactory;
        private readonly BotsNicknamesProviderService _botsNicknamesProviderService;
        private readonly BotsCustomizationDataProviderService _botsCustomizationDataProviderService;

        public BotsClientSynchronizer(BotsFactory botsFactory,
            EmotionsConfigsProviderService emotionsConfigsProviderService, EmotionsViewFactory emotionsViewFactory,
            BotsNicknamesProviderService botsNicknamesProviderService,
            BotsCustomizationDataProviderService botsCustomizationDataProviderService)
        {
            _botsFactory = botsFactory;
            _emotionsConfigsProviderService = emotionsConfigsProviderService;
            _emotionsViewFactory = emotionsViewFactory;
            _botsNicknamesProviderService = botsNicknamesProviderService;
            _botsCustomizationDataProviderService = botsCustomizationDataProviderService;
        }

        public void InitializeBot(InitializationBotBroadcast broadcast, Channel channel)
        {
            var botObjectId = broadcast.BotGameObject.GetComponent<NetworkObject>().ObjectId;

            _botsNicknamesProviderService.AddBotNickname(
                botObjectId, broadcast.Nickname);

            _botsCustomizationDataProviderService.AddBotCustomizationData(botObjectId,
                broadcast.SerializedBotCustomizationData.GetDeserializedWithoutNulls<CustomizationData>());

            _botsFactory.InitializeBotAsync(broadcast.BotGameObject.GetComponent<SharedBotSerializableComponents>());
        }

        public void ChangeBotNickname(BotChangedNicknameBroadcast broadcast, Channel channel) =>
            _botsNicknamesProviderService.TrySettingBotNickname(broadcast.BotObjectId, broadcast.NewNickname);
        
        public void ChangeBotCustomizationData(BotChangedCustomizationDataBroadcast broadcast, Channel channel) =>
            _botsCustomizationDataProviderService.TrySettingBotCustomizationData(broadcast.BotObjectId, broadcast.NewSerializedData.GetDeserializedWithoutNulls<CustomizationData>());

        public async void PlayBotEmotionAsync(PlayBotEmotionBroadcast broadcast, Channel channel)
        {
            EmotionsAnimatorController emotionAnimatorController = null;

            await AsyncTools.WaitWhileWithoutSkippingFrame(() =>
                (emotionAnimatorController =
                    _botsFactory.BotsEmotionsAnimatorControllers.GetValueOrDefault(broadcast.BotGameObject)) == null);

            var emotionsComponents = _botsFactory.BotsEmotionsAnimatorControllers[broadcast.BotGameObject];

            var particlesParents = await _emotionsViewFactory.TryCreatingParticlesParentsAsync(
                emotionsComponents.ParticleEventController.transform, broadcast.EmotionIndex);

            emotionsComponents.ParticleEventController.UpdateParticleObjects(particlesParents
                .Select(parent => parent.gameObject).ToArray());

            emotionAnimatorController.SetAnimatorControllerAndTryStartingEmotionAnimation(
                (await _emotionsConfigsProviderService.GetEmotionsAnimationsConfig()).EmotionsAnimationConfigs
                .FirstOrDefault(config => config.EmotionIndex == broadcast.EmotionIndex)
                ?.OverrideController, broadcast.EmotionIndex);
        }
    }
}