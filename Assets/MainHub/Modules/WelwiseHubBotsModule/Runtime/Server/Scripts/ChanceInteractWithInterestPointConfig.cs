using System;
using UnityEngine;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    [Serializable]
    public class ChanceInteractWithInterestPointConfig
    {
        [field: SerializeField] [field: Range(1, 100)] public float Chance { get; private set; }
        [field: SerializeField] public InterestPointGroup Group { get; private set; }
    }
}