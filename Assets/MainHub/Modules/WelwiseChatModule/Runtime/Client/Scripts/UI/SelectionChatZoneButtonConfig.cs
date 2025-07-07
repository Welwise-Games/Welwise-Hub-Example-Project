using System;
using UnityEngine;
using UnityEngine.UI;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI
{
    [Serializable]
    public class SelectionChatZoneButtonConfig
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public ChatZone Zone { get; private set;  }
    }
}