using System;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Client.Scripts.Tools
{
    [Serializable]
    public class AudioClipWithPitchRange
    {
        [field: SerializeField]
        [field: Range(0, 2)]
        public float MinimalPitch { get; private set; } = 0.99f;

        [field: SerializeField]
        [field: Range(0, 2)]
        public float MaximumPitch { get; private set; } = 1.01f;

        [field: SerializeField] public AudioClip Clip { get; private set; }
    }
}