using UnityEngine;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Tools;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "WelwiseMiniGamesModule/DoodleJump/Item")]
    public class Item : ScriptableObject
    {
        public GameObject prefab;
        [MinMaxRange(-1f, 1f)]
        public RangedFloat xRange;
    }
}
