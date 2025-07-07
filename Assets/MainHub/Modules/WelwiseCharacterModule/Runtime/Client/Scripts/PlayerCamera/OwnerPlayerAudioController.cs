using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement;
using WelwiseCharacterModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.AnimationEventSystem;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.PlayerCamera
{
    public class OwnerPlayerAudioController
    {
        public OwnerPlayerAudioController(OwnerPlayerMovementController movementController,
            OwnerPlayerMovementSerializableComponents ownerPlayerMovementSerializableComponents,
            AudioSource walkingAudioSource, AudioSource jumpingAudioSource,
            HeroAudioClipsProviderService heroAudioClipsProviderService, AnimatorStateObserver animatorStateObserver,
            Animator animator)
        {
            movementController.Jumped += PlayJumpSoundAsync;

            new AnimationsActionsInvoker(animatorStateObserver,
                ownerPlayerMovementSerializableComponents.AnimationFramesWhenShouldPlayWalkingSounds.Select(frame =>
                        new AnimationActionInfo(() => frame, animator, PlayWalkingSoundAsync)).ToList(), HeroAnimatorController.RunHash);

            async void PlayWalkingSoundAsync()
            {
                var clip = (await heroAudioClipsProviderService.GetHeroAudioClipsConfigAsync()).WalkingOnGroundClip;
                walkingAudioSource.SetPitchAndPlayOneShot(clip);
            }

            async void PlayJumpSoundAsync()
            {
                jumpingAudioSource.SetPitchAndPlayOneShot(
                    (await heroAudioClipsProviderService.GetHeroAudioClipsConfigAsync()).JumpClip);
            }
        }
    }
}