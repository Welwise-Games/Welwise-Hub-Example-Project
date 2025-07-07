using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.InputServices
{
    public struct CameraInputData
    {
        public readonly Vector2 InputAxis;
        public readonly bool IsHold;

        public CameraInputData(bool isHold, Vector2 inputAxis)
        {
            IsHold = isHold;
            InputAxis = inputAxis;
        }
    }
}