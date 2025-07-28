using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.Helicopter.Scripts
{
    public class Package : MonoBehaviour
    {
        public AudioSource AudioSourceFX;
        private void Start() {
            AudioSourceFX.volume = PlayerPrefs.GetInt("FXMusic");
        }
        private void OnCollisionEnter2D(Collision2D other) {
            AudioSourceFX.Play();
        }
    }
}
