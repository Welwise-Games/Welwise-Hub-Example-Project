using System;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement
{
    public class OwnerPlayerMovementController
    {
        public float VerticalVelocity { get; private set; }

        public bool IsEnabled { get; set; } = true;

        public event Action Jumped, MovedOnGround, NotMovedOnGround;
        public event Action<Vector3> Moved;

        private readonly Transform _cameraTransform;
        private readonly OwnerPlayerMovementSerializableComponents _serializableComponents;
        private readonly CharacterController _characterController;
        private readonly IInputService _inputService;

        public OwnerPlayerMovementController(Transform cameraTransform,
            OwnerPlayerMovementSerializableComponents serializableComponents, IInputService inputService,
            CharacterController characterController)
        {
            _cameraTransform = cameraTransform;
            _serializableComponents = serializableComponents;
            _inputService = inputService;
            _characterController = characterController;

            serializableComponents.MonoBehaviourObserver.Updated += OnUpdate;
        }

        public void TryJumping()
        {
            if (!_characterController.isGrounded) return;

            VerticalVelocity = _serializableComponents.MovementConfig.JumpForce;

            Jumped?.Invoke();
        }

        private void OnUpdate()
        {
            if (_inputService.ShouldJump())
                TryJumping();

            HandleGravity();

            var inputAxis = !IsEnabled
                ? Vector3.zero
                : _inputService.GetInputAxis();
            
            Move(inputAxis, out var movementDelta);

            TryRotating(movementDelta);
        }

        private void HandleGravity() =>
            VerticalVelocity += _serializableComponents.MovementConfig.GravityForce * Time.deltaTime;

        private void Move(Vector3 direction, out Vector3 movementDelta)
        {
            var forwardDirection = new Vector3(_cameraTransform.forward.x, 0f, _cameraTransform.forward.z).normalized;
            var rightDirection = new Vector3(_cameraTransform.right.x, 0f, _cameraTransform.right.z).normalized;
            var movementDirection = rightDirection * direction.x + forwardDirection * direction.z;

            movementDelta = movementDirection * (_serializableComponents.MovementConfig.MoveSpeed * Time.deltaTime);

            var finalMovement = new Vector3(movementDelta.x, VerticalVelocity * Time.deltaTime, movementDelta.z);

            _characterController.Move(finalMovement);

            Moved?.Invoke(direction);

            if (direction.magnitude != 0 && _characterController.isGrounded)
                MovedOnGround?.Invoke();
            else
                NotMovedOnGround?.Invoke();
        }

        private void TryRotating(Vector3 direction)
        {
            if (direction.magnitude == 0) return;

            var targetRotation = Quaternion.LookRotation(direction);
            _characterController.transform.rotation =
                Quaternion.Lerp(_characterController.transform.rotation, targetRotation,
                    _serializableComponents.MovementConfig.RotationSpeed * Time.deltaTime);
        }
    }
}