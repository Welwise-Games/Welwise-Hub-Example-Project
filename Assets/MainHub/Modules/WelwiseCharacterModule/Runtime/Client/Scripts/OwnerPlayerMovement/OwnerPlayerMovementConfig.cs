using System;
using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement
{
    [Serializable]
    public class OwnerPlayerMovementConfig
    {
        [field: SerializeField] [field: Range(0.1f, 100)] public float MoveSpeed { get; private set; } = 10f;
        [field: SerializeField] [field: Range(10, 100)] public float RotationSpeed { get; private set; } = 30f;
        [field: SerializeField] [field: Range(0.1f, 10f)] public float JumpForce { get; private set; } = 2;
        [field: SerializeField] [field: Range(-30, -10)] public float GravityForce { get; private set; } = -10;
    }
}