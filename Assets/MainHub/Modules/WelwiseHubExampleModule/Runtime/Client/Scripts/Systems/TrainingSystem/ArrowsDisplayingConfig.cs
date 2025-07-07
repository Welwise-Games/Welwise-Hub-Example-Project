using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    [CreateAssetMenu(menuName = "Training/ArrowsDisplayingConfig", fileName = "ArrowsDisplayingConfig")]
    public class ArrowsDisplayingConfig : ScriptableObject
    {
        [field: SerializeField] [field: Range(0.1f, 10)] public float DistanceBetweenArrows { get; private set; } = 1;
        [field: SerializeField] [field: Range(0.1f, 10)] public float ArrowsMovementSpeed { get; private set; } = 1;
        [field: SerializeField] [field: Range(0.1f, 10)] public float ArrowsMovementAmplitude { get; private set; } = 2;
        [field: SerializeField] [field: Range(0.1f, 10)] public float ArrowsOffsetY { get; private set; } = 1;
    }
}