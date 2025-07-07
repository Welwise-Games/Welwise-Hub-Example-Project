using System;
using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    [Serializable]
    public class DefaultClothesInstances
    {
        [field: SerializeField] public ItemCategory ItemCategory { get; private set; }
        [field: SerializeField] public GameObject[] Instances { get; private set; }
    }
}