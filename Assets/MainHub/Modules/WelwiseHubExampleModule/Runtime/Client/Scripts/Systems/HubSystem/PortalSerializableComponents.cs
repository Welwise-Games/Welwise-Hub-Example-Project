using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class PortalSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Image IconImage { get; private set; }
        [field: SerializeField] public Image CoverImage { get; private set; }
        [field: SerializeField] public VideoPlayer VideoPlayer { get; private set; }
        [field: SerializeField] public RawImage VideoRawImage { get; private set; }
        [field: SerializeField] public ColliderObserver ShowingVideoZoneColliderObserver { get; private set; }
        [field: SerializeField] public ColliderObserver EnteringGameZoneColliderObserver { get; private set; }
        [field: SerializeField] public AudioSource MusicAudioSource { get; private set; }
    }
}