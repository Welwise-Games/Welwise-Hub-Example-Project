using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.Shared.Scripts
{
    public class MiniGameSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public GotRewardProvider GotRewardProvider { get; private set; }
    }
}