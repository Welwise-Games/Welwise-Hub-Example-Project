using System;
using UnityEngine;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    [Serializable]
    public class EmotionViewConfig
    {
        [field: SerializeField] [field: Range(0, 120)] public int FrameWhenPlayParticles { get; private set; }
        [field: SerializeField] public string EmotionIndex { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public ParticlesParentSerializableComponents[] ParticlesComponentsParentsPrefabs { get; private set; }
    }
}