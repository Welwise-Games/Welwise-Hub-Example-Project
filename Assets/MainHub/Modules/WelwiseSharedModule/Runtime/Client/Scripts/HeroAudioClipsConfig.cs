using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;

namespace WelwiseSharedModule.Runtime.Client.Scripts
{
    [CreateAssetMenu(menuName = "WelwiseHubExampleModule/HeroAudioClipsConfig")]
    public class HeroAudioClipsConfig : ScriptableObject
    {
        [field: SerializeField] public AudioClipWithPitchRange JumpClip { get; private set; }
        [field: SerializeField] public AudioClipWithPitchRange WalkingOnGroundClip { get; private set; }
    }
}