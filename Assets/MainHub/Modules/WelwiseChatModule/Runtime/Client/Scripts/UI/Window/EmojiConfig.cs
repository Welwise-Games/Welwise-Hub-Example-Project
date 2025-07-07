using System;
using UnityEngine;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI.Window
{
    [Serializable]
    public class EmojiConfig
    {
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public int Index { get; private set; }
    }
}