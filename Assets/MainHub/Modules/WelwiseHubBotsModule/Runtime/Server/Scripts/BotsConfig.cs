using System.Collections.Generic;
using UnityEngine;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    [CreateAssetMenu(fileName = "BotsConfig", menuName = "WelwiseHubBotsModule/BotsConfig")]
    public class BotsConfig : ScriptableObject
    {
        [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
        [field: SerializeField] [field: Range(0, 100)] public int MaximumBotsNumber { get; private set; } = 6;
        [field: SerializeField] [field: Range(1, 44)] public float MinimalInterestPointChangingTime { get; private set; } = 15;
        [field: SerializeField] [field: Range(2, 120)] public float MaximumInterestPointChangingTime { get; private set; } = 45;
        [field: SerializeField] [field: Range(1, 1200)] public float MinimalBotRespawnTime { get; private set; } = 60;
        [field: SerializeField] [field: Range(2, 3600)] public float MaximumBotRespawnTime{ get; private set; } = 180;
        [field: SerializeField] [field: Range(1, 60)] public float MinimalEmotionAnimationWaitingTime { get; private set; } = 60;
        [field: SerializeField] [field: Range(1, 500)] public float MaximumEmotionAnimationWaitingTime { get; private set; } = 360;
        [field: SerializeField] [field: Range(1, 60)] public float MinimalChangingClothAndNicknameTime { get; private set; } = 10;
        [field: SerializeField] [field: Range(1, 500)] public float MaximumChangingClothAndNicknameTime { get; private set; } = 120;
        [field: SerializeField]  [field: Range(0, 100)] public float SetBotCustomizationDataPartChance { get; private set; } = 50;  
        [field: SerializeField] [field: Range(1, 10)] public int MaxUniformInterestPointGroupsInRow { get; private set; } = 3;
        [field: SerializeField] public List<ChanceInteractWithInterestPointConfig> ChanceInteractWithInterestPointConfigs { get; private set; }
    }
}