using System;
using UnityEngine;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI.Window
{
    [Serializable]
    public class ChatPopupSettingScaleModeButtonConfig
    {
        [field: SerializeField] public float PositionY { get; private set; }
        [field: SerializeField] public float Height { get; private set; }
    }
}