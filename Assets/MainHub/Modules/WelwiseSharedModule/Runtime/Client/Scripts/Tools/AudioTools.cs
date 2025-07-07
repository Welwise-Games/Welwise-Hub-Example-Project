using UnityEngine;

namespace WelwiseSharedModule.Runtime.Client.Scripts.Tools
{
    public static class AudioTools
    {
        public static void SetPitchAndPlayOneShot(this AudioSource audioSource, AudioClipWithPitchRange audioClipWithPitchRange)
        {
            var startPitch = audioSource.pitch;
            audioSource.pitch = Random.Range(audioClipWithPitchRange.MinimalPitch, audioClipWithPitchRange.MaximumPitch);
            audioSource.PlayOneShot(audioClipWithPitchRange.Clip);
            audioSource.pitch = startPitch;
        }
        
        public static void SetPitchAndPlay(this AudioSource audioSource, AudioClipWithPitchRange audioClipWithPitchRange)
        {
            audioSource.pitch = Random.Range(audioClipWithPitchRange.MinimalPitch, audioClipWithPitchRange.MaximumPitch);
            
            if (audioSource.isPlaying && audioSource.clip == audioClipWithPitchRange.Clip)
                return;
            
            audioSource.clip = audioClipWithPitchRange.Clip;
            audioSource.Play();
        }
    }
}