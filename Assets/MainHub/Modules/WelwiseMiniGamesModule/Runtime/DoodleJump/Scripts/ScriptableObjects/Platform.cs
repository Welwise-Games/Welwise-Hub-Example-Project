using UnityEngine;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Tools;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "WelwiseMiniGamesModule/DoodleJump/Platform")]
    public class Platform : ScriptableObject
    {
        public GameObject prefab;
        public float difficulty;
        public bool hasItem;
        
        [MinMaxRange(-2, 2)]
        public RangedFloat xRange;
        [MinMaxRange(0.5f, 3.9f)]
        public RangedFloat yRange;
    }
}
