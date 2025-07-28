using System;
using System.Linq;
using Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class ClientHubSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public SlotMachineView[] SlotMachinesViews { get; private set; }
        [field: SerializeField] public ShopSerializableComponents ShopSerializableComponents { get; private set; }
        [field: SerializeField] public PortalSerializableComponents[] PortalsSerializableComponents { get; private set; }
        [field: SerializeField] public AnimatedWithEmotionHeroSerializableComponents[] AnimatedWithEmotionHeroSerializableComponents { get; private set; }
        [field: SerializeField] public SetPlayerAnimationPlaceSerializableComponents[] SetPlayerAnimationAndPositionAndRotationButtonsSerializableComponents {get; private set; }

        private void OnValidate()
        {
            SlotMachinesViews = GetComponentsInChildren<SlotMachineView>();
            ShopSerializableComponents = GetComponentInChildren<ShopSerializableComponents>();
            PortalsSerializableComponents = GetComponentsInChildren<PortalSerializableComponents>();
            AnimatedWithEmotionHeroSerializableComponents =
                GetComponentsInChildren<AnimatedWithEmotionHeroSerializableComponents>();
            SetPlayerAnimationAndPositionAndRotationButtonsSerializableComponents =
                GetComponentsInChildren<SetPlayerAnimationPlaceSerializableComponents>();
        }
    }
}