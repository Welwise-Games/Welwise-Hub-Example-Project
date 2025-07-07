using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick
{
    public class JoystickHighlightersPartsController
    {
        public JoystickHighlightersPartsController(
            JoystickHighlightedPartsSerializableComponents joystickHighlightedPartsSerializableComponents,
            JoystickController joystickController)
        {
            joystickHighlightedPartsSerializableComponents.PointerDragObserver.DragedWithoutArgs += 
                () => SetActiveHighlightedParts(joystickHighlightedPartsSerializableComponents, joystickController);
        }

        private void SetActiveHighlightedParts(JoystickHighlightedPartsSerializableComponents serializableComponents, JoystickController joystickController)
        {
            serializableComponents.JoystickHighlightedPartSerializableComponents.ForEach(components =>
                components.Image.gameObject.SetActive(IsEnabledPart(components.JoystickDirection,
                    joystickController)));
        }

        private bool IsEnabledPart(JoystickDirection joystickDirection, JoystickController joystickController)
        {
            var inputAxis = joystickController.InputAxis;

            switch (joystickDirection)
            {
                case JoystickDirection.LeftBottom:
                    return inputAxis is { x: < 0, y: < 0 };
                case JoystickDirection.RightBottom:
                    return inputAxis is { x: > 0, y: < 0 };
                case JoystickDirection.LeftTop:
                    return inputAxis is { x: < 0, y: > 0 };
                case JoystickDirection.RightTop:
                    return inputAxis is { x: > 0, y: > 0 };
                default:
                    return false;
            }
        }
    }
}