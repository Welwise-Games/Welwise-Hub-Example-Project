using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    [CreateAssetMenu(fileName = "PortalsConfig", menuName = "PortalsConfig")]
    public class PortalsConfig : ScriptableObject
    {
        [field: SerializeField] public string IconFileName { get; private set; } = "Icon.png";
        [field: SerializeField] public string CoverFileName { get; private set; } = "Cover.png";
        [field: SerializeField] public string VideoFileName { get; private set; } = "Video.mp4";
        [field: SerializeField] public string GameIdFileName { get; private set; }  = "GameId.txt";
        [field: SerializeField] public string MaterialsUrl { get; private set; } = "https://github.com/Welwise-Games/PromoForGamesInMainHub";
        [field: SerializeField] public AudioClipWithPitchRange EnterPortalAudioClip { get; private set; }
    }
}