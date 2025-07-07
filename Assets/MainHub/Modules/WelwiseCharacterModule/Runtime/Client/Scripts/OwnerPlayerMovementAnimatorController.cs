using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.HeroAnimators;
using WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement;
using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerCamera;
using WelwiseCharacterModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseCharacterModule.Runtime.Client.Scripts
{
    public class OwnerPlayerMovementAnimatorController
    {
        public OwnerPlayerMovementAnimatorController(OwnerPlayerMovementController ownerPlayerMovementController,
            MonoBehaviourObserver monoBehaviourObserver,
            CameraController cameraController, HeroAnimatorController heroAnimatorController,
            ArmsAnimatorController armsAnimatorController, CharacterController characterController)
        {
            ownerPlayerMovementController.Jumped += TriggerJump;
            ownerPlayerMovementController.Moved += HandleIsRunning;
            monoBehaviourObserver.Updated += HandleIsFalling;

            void HandleIsRunning(Vector3 direction)
            {
                heroAnimatorController.SetIsRunning(direction.magnitude != 0);
                armsAnimatorController.SetIsRunning(direction.magnitude != 0);
            }

            void HandleIsFalling()
            {
                heroAnimatorController.SetIsFalling(
                    !characterController.isGrounded &&
                    ownerPlayerMovementController.VerticalVelocity < 0);
            }

            void TriggerJump()
            {
                if (cameraController is not { IsFirstCameraMode: true })
                {
                    heroAnimatorController.TriggerJump();
                }
                else
                {
                    armsAnimatorController.TriggerJump();
                }
            }
        }
    }
}