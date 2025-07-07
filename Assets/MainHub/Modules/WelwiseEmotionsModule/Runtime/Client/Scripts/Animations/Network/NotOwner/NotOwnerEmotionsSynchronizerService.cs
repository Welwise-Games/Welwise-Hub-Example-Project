using System.Collections.Generic;
using System.Linq;
using FishNet.Managing.Client;
using FishNet.Transporting;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network.Dependencies;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.NotOwner
{
    public class NotOwnerEmotionsSynchronizerService
    {
        private readonly INotOwnerPlayersComponentsProviderService _animatorsProvidersService;
        private readonly EmotionsAnimationsConfig _emotionsAnimationsConfig;
        private readonly EmotionsViewFactory _emotionsViewFactory;

        public NotOwnerEmotionsSynchronizerService(
            INotOwnerPlayersComponentsProviderService animatorsProvidersService,
            EmotionsAnimationsConfig emotionsAnimationsConfig, ClientManager clientManager, EmotionsViewFactory emotionsViewFactory)
        {
            _animatorsProvidersService = animatorsProvidersService;
            _emotionsAnimationsConfig = emotionsAnimationsConfig;
            _emotionsViewFactory = emotionsViewFactory;

            clientManager.RegisterBroadcast<PlayingEmotionAnimationDependenciesForClient>(
                HandlePlayingAnimationAsync);
        }

        private async void HandlePlayingAnimationAsync(PlayingEmotionAnimationDependenciesForClient dependencies,
            Channel channel)
        {
            var playerEmotionsComponents = _animatorsProvidersService.EmotionsComponents
                .GetValueOrDefault(dependencies.PlayingPlayerNetworkConnection);

            var particlesParents = await _emotionsViewFactory.TryCreatingParticlesParentsAsync(
                playerEmotionsComponents.ParticleEventController.transform, dependencies.EmotionIndex);

            playerEmotionsComponents.ParticleEventController.UpdateParticleObjects(particlesParents.Select(parent => parent.gameObject).ToArray());
            
            playerEmotionsComponents.EmotionsAnimatorController.SetAnimatorControllerAndTryStartingEmotionAnimation(
                    _emotionsAnimationsConfig.EmotionsAnimationConfigs
                        .FirstOrDefault(config => config.EmotionIndex == dependencies.EmotionIndex)
                        ?.OverrideController, dependencies.EmotionIndex);
        }
    }
}