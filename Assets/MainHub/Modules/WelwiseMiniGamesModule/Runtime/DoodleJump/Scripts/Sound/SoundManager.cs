using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Sound
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource source;
        public AudioClip jumpSound;
        public AudioClip deathSound;

        public void PlayJumpSound()
        {
            source.clip = jumpSound;
            source.Play();
        }

        public void PlayDeathSound()
        {
            source.clip = deathSound;
            source.Play();
        }
    }
}
