using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Game
{
    public class MapUnloader : MonoBehaviour
    {
        public Transform platformParent;
        public Transform player;
        public float unloadDistance = 10f;

        public void Update()
        {
            var playerPos = player.position;

            foreach (Transform platform in platformParent)
            {
                if (platform.position.y < playerPos.y - unloadDistance)
                {
                    Destroy(platform.gameObject);
                }
            }
        }
    }
}
