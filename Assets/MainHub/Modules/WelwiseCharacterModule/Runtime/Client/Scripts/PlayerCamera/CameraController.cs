using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.PlayerCamera
{
    public class CameraController
    {
        public float DistanceBetweenPlayerAndCamera => Vector3.Distance(_camera.transform.position, _playerTransform.position);
        public bool IsFirstCameraMode { get; private set; }

        public event Action<bool> ChangedCameraMode;

        private float _currentHorizontalAngle;
        private float _additionalLookUpAngle;
        private float _currentVerticalAngle = 20f;
        private float _currentZoom = 1f;
        private float _targetZoom = 1f;

        private readonly Camera _camera;
        private readonly Transform _playerTransform;
        private readonly CameraControllerSerializableComponents _serializableComponents;
        private readonly IInputService _inputService;
        private readonly List<Func<bool>> _canSwitchCameraModeFuncs = new List<Func<bool>>();

        public CameraController(Transform playerTransform, Camera camera,
            CameraControllerSerializableComponents serializableComponents, IInputService inputService)
        {
            _playerTransform = playerTransform;
            _camera = camera;
            _serializableComponents = serializableComponents;
            _inputService = inputService;

            serializableComponents.MonoBehaviourObserver.LateUpdated += OnLateUpdate;
            serializableComponents.MonoBehaviourObserver.Updated += OnUpdate;
        }

        public void AddCanSwitchCameraModeFunc(Func<bool> func) => _canSwitchCameraModeFuncs.Add(func);

        private void OnUpdate()
        {
            var cameraInputData = _inputService.GetCameraInputData();

            if (_canSwitchCameraModeFuncs.All(func => func.Invoke()) && _inputService.ShouldSwitchCameraMode())
                SwitchCameraMode();

            if (cameraInputData.IsHold && CursorSwitchTools.IsCursorEnabled || !CursorSwitchTools.IsCursorEnabled)
                TryRotating(cameraInputData.InputAxis);
        }

        private void SwitchCameraMode()
        {
            IsFirstCameraMode = !IsFirstCameraMode;
            ChangedCameraMode?.Invoke(IsFirstCameraMode);
           
            if (IsFirstCameraMode)
                CursorSwitchTools.TryDisablingCursor();
        }

        private void OnLateUpdate()
        {
            if (IsFirstCameraMode)
            {
                UpdateFirstPersonCamera();
            }
            else
            {
                UpdateThirdPersonCamera();
            }
        }

        private void UpdateFirstPersonCamera()
        {
            _camera.transform.position = _serializableComponents.CameraConfig.FpsCameraPosition.position;

            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, -90f, 90f);
            var rotation = Quaternion.Euler(_currentVerticalAngle, _currentHorizontalAngle, 0);

            _camera.transform.rotation = rotation;
            _serializableComponents.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }

        private void UpdateThirdPersonCamera()
        {
            _currentZoom = _targetZoom;

            var rotation = Quaternion.Euler(_currentVerticalAngle, _currentHorizontalAngle, 0);
            var desiredPosition =
                _playerTransform.position + rotation * (_serializableComponents.CameraConfig.Offset * _currentZoom);

            var zoomFactor =
                Mathf.InverseLerp(
                    _serializableComponents.CameraConfig.MinimumZoomDistance /
                    _serializableComponents.CameraConfig.Offset.magnitude, 1f, _currentZoom);
            _additionalLookUpAngle =
                Mathf.Lerp(0f, _serializableComponents.CameraConfig.MaximumLookUpAngle, 1f - zoomFactor);
            _camera.transform.position = desiredPosition;
            _camera.transform.LookAt(_playerTransform.position + Vector3.up * (_additionalLookUpAngle * 0.1f));
        }

        private void TryRotating(Vector2 inputAxis)
        {
            if (Mathf.Approximately(inputAxis.x, 0f) && Mathf.Approximately(inputAxis.y, 0f))
                return;

            _currentHorizontalAngle += inputAxis.x * _serializableComponents.CameraConfig.RotationSpeed;
            _currentVerticalAngle += inputAxis.y * _serializableComponents.CameraConfig.RotationSpeed;

            _currentVerticalAngle = IsFirstCameraMode
                ? Mathf.Clamp(_currentVerticalAngle, -75, 45)
                : Mathf.Clamp(_currentVerticalAngle, _serializableComponents.CameraConfig.MinimumVerticalAngle,
                    _serializableComponents.CameraConfig.MaximumVerticalAngle);

            if (_currentVerticalAngle <= _serializableComponents.CameraConfig.MinimumVerticalAngle)
            {
                switch (inputAxis.y)
                {
                    case < 0:
                        _targetZoom -= Mathf.Abs(inputAxis.y) * _serializableComponents.CameraConfig.ZoomSpeed *
                                       Time.deltaTime;
                        break;
                    case > 0:
                        _targetZoom += inputAxis.y * _serializableComponents.CameraConfig.ZoomSpeed * Time.deltaTime;
                        break;
                }

                _targetZoom = Mathf.Clamp(_targetZoom,
                    _serializableComponents.CameraConfig.MinimumZoomDistance /
                    _serializableComponents.CameraConfig.Offset.magnitude, 1f);
            }
            else if (_currentVerticalAngle > _serializableComponents.CameraConfig.MinimumVerticalAngle &&
                     inputAxis.y > 0 &&
                     _currentZoom < 1f)
            {
                _targetZoom += inputAxis.y * _serializableComponents.CameraConfig.ZoomSpeed * Time.deltaTime;
                _targetZoom = Mathf.Min(_targetZoom, 1f);
            }
        }
    }
}