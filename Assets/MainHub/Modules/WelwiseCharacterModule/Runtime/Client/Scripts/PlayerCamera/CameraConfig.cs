using System;
using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.PlayerCamera
{
    [Serializable]
    public class CameraConfig
    {
        [field: Header("Camera Settings")] 
        [field: SerializeField] public Vector3 Offset { get; private set; } = new(0, 5, -9);
        [field: SerializeField] public Transform FpsCameraPosition { get; private set; }
        [field: SerializeField] public float RotationSpeed { get; private set; } = 5f;
        [field: SerializeField] [field: Range(-40, -10)] public float MinimumVerticalAngle { get; private set; } = -20f;
        [field: SerializeField] [field: Range(10, 60)] public float MaximumVerticalAngle { get; private set; } = 40;

        [field: Header("Zoom Settings")]
        [field: SerializeField] [field: Range(0.1f, 10f)] public float ZoomSpeed { get; private set; } = 7f;
        [field: SerializeField] [field: Range(0.1f, 10f)] public float MinimumZoomDistance { get; private set; } = 2f;
        [field: SerializeField] [field: Range(30, 60)] public float MaximumLookUpAngle { get; private set; } = 45;
    }
}