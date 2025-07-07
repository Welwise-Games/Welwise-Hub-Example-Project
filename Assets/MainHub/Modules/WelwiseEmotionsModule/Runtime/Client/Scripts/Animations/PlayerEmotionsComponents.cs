using FishNet.Component.Animating;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations
{
    public class PlayerEmotionsComponents
    {
        public PlayerEmotionsSerializableComponents SerializableComponents { get; }
        public IEmotionsAnimatorController EmotionsAnimatorController { get; }
        public AnimatorStateObserver AnimatorStateObserver { get; }
        public ParticleEventController ParticleEventController { get; }

        public PlayerEmotionsComponents(PlayerEmotionsSerializableComponents serializableComponents,
            EmotionsViewConfig emotionsViewConfig, NetworkAnimator networkAnimator,
            AnimatorStateObserver animatorStateObserver, ParticleEventController particleEventController)
        {
            SerializableComponents = serializableComponents;
            AnimatorStateObserver = animatorStateObserver;
            ParticleEventController = particleEventController;

            var emotionsAnimatorController = new EmotionsAnimatorController(serializableComponents.Animator,
                animatorStateObserver, particleEventController, emotionsViewConfig, networkAnimator);

            EmotionsAnimatorController =
                emotionsAnimatorController;
        }
    }
}