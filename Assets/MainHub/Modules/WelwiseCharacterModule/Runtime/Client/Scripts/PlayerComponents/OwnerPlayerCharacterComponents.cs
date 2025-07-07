using MainHub.Modules.WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents;
using WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement;
using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerCamera;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents
{
    public class OwnerPlayerCharacterComponents
    {
        public readonly OwnerPlayerCharacterSerializableComponents CharacterSerializableComponents;
        public readonly ClientPlayerCharacterComponents ClientCharacterComponents;
        public readonly OwnerPlayerMovementController MovementController;
        public readonly CursorController CursorController;
        public readonly CameraController CameraController;

        public OwnerPlayerCharacterComponents(OwnerPlayerCharacterSerializableComponents characterSerializableComponents, 
            OwnerPlayerMovementController movementController, ClientPlayerCharacterComponents clientCharacterComponents, CursorController cursorController, CameraController cameraController)
        {
            CharacterSerializableComponents = characterSerializableComponents;
            MovementController = movementController;
            ClientCharacterComponents = clientCharacterComponents;
            CursorController = cursorController;
            CameraController = cameraController;
        }
    }
}