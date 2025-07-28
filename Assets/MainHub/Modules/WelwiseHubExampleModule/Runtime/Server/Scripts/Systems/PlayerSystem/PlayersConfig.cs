using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.PlayerSystem
{
    [CreateAssetMenu(menuName = "WelwiseHubExampleModule/PlayersConfig")]
    public class PlayersConfig : ScriptableObject
    {
        [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
    }
}