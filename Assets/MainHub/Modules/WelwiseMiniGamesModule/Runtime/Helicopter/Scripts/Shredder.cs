using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.Helicopter.Scripts
{
    public class Shredder : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other) {
            Destroy(other.gameObject);
        }
    }
}
