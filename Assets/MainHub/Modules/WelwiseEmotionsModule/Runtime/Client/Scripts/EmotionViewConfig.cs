using System;
using UnityEngine;
using WelwiseItemInShopModule.Client.Scripts;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    [Serializable]
    public class EmotionViewConfig : IItemViewConfig
    {
        [field: SerializeField] [field: Range(0, 120)] public int FrameWhenPlayParticles { get; private set; }
        [field: SerializeField] public string ItemIndex { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public ParticlesParentSerializableComponents[] ParticlesComponentsParentsPrefabs { get; private set; }
    }
}