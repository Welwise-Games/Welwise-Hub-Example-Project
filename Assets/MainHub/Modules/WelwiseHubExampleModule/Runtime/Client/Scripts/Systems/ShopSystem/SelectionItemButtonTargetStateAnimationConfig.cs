using System;
using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    [Serializable]
    public class SelectionItemButtonTargetStateAnimationConfig
    {
        [field: SerializeField] [field: Range(0, 3)] public float SpeedChangingScaleOnSetTargetState { get; private set; } = 1;
        [field: SerializeField] [field: Range(1, 2)] public float ScaleMultiplierOnBecomeTarget { get; private set; } = 1.25f;
    }
}