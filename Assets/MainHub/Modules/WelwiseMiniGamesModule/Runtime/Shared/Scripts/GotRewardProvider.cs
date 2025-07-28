using System;
using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.Shared.Scripts
{
    public class GotRewardProvider : MonoBehaviour
    {
        public event Action Got;

        public void ClearEvent() => Got = null;
        public void InvokeGot() => Got?.Invoke();
    }
}