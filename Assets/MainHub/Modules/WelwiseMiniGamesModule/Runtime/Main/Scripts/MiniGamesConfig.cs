using UnityEngine;

namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    [CreateAssetMenu(menuName = "WelwiseMiniGamesModule/MiniGamesConfig")]
    public class MiniGamesConfig : ScriptableObject
    {
        [field: SerializeField] public MiniGameConfig[] MiniGamesConfigs { get; private set; }
    }
}