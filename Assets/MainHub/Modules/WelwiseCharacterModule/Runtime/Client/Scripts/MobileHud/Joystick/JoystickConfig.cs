using System;
using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick
{
    [Serializable]
    public class JoystickConfig
    {
        [field: SerializeField] public float HandleRange { get; private set; } = 1f;
        [field: SerializeField] public bool IsDynamic { get; private set; } = true;
        [field: SerializeField] public float FadeDuration { get; private set; } = 1f;
    }
}