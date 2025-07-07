using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.PlayerSystem
{
    [CreateAssetMenu(menuName = "PlayersConfig", fileName = "PlayersConfig")]
    public class PlayersConfig : ScriptableObject
    {
        [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
    }
}