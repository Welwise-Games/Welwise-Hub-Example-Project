using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class ClientHubSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public ShopSerializableComponents ShopSerializableComponents { get; private set; }
        [field: SerializeField] public PortalSerializableComponents[] PortalsSerializableComponents { get; private set; }
        [field: SerializeField] public AnimatedWithEmotionHeroSerializableComponents[] AnimatedWithEmotionHeroSerializableComponents { get; private set; }
        [field: SerializeField] public SetPlayerAnimationPlaceSerializableComponents[] SetPlayerAnimationAndPositionAndRotationButtonsSerializableComponents {get; private set;}
    }
}