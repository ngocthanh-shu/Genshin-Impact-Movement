using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerMovementState : IState
    {
        protected PlayerMovementStateMachine _playerMovementStateMachine;
        
        protected PlayerGroundedData _playerGroundedData;
        protected PlayerAirborneData _playerAirborneData;

        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            _playerMovementStateMachine = playerMovementStateMachine;
            
            _playerGroundedData = _playerMovementStateMachine.Player.Data.GroundedData;
            _playerAirborneData = _playerMovementStateMachine.Player.Data.AirborneData;

            InitializeData();
        }

        private void InitializeData()
        {
            SetBaseRotationData();
        }

        #region IState Methods
        public virtual void Enter()
        {
            Debug.Log("State: " + GetType().Name + " entered.");
            //Debug.Log("Should walk: " + _playerMovementStateMachine.ReusableData.ShouldWalk);
            
            AddInputActionCallbacks();
        }

        public virtual void Exit()
        {
            RemoveInputActionCallbacks();
        }

        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void Update()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            Move();
        }

        public virtual void OnAnimationEnterEvent()
        {
            
        }

        public virtual void OnAnimationExitEvent()
        {
            
        }

        public virtual void OnAnimationTransitionEvent()
        {
            
        }

        public void OnTriggerEnter(Collider colliders)
        {
            if (_playerMovementStateMachine.Player.LayerData.IsGroundLayer(colliders.gameObject.layer))
            {
                OnContactWithGround(colliders);
                
                return;
            }
        }

        public void OnTriggerExit(Collider colliders)
        {
            if (_playerMovementStateMachine.Player.LayerData.IsGroundLayer(colliders.gameObject.layer))
            {
                OnContactWithGroundExited(colliders);
                
                return;
            }
        }

        #endregion

        #region Main Methods

        private void ReadMovementInput()
        {
            _playerMovementStateMachine.ReusableData.MovementInput = _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.ReadValue<Vector2>();
        }

        private void Move()
        {
            if(_playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero || _playerMovementStateMachine.ReusableData.MovementSpeedModifier == 0) return;
            Vector3 movementDirection = GetMovementInputDirection();
            
            float targetRotationYAngle = Rotate(movementDirection);
            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float movementSpeed = GetMovementSpeed();
            
            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
            
            _playerMovementStateMachine.Player.Rigidbody
                .AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, 
                    ForceMode.VelocityChange);
            
        }

        private float Rotate(Vector3 direction)
        {
            var directionAngle = UpdateTargetRotation(direction);

            RotateTowardsTargetRotation();
            
            return directionAngle;
        }

        private float AddCameraRotationToAngle(float angle)
        {
            angle += _playerMovementStateMachine.Player.CameraTransform.eulerAngles.y;
            
            if(angle > 360f) angle -= 360f;
            
            return angle;
        }

        private float GetDirectionAngle(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            
            if(directionAngle < 0f) directionAngle += 360f;
            
            return directionAngle;
        }
        
        private void UpdateTargetRotationData(float targetAngle)
        {
            _playerMovementStateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;
            _playerMovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0;
        }

        #endregion
        
        #region Reusable Methods
        protected Vector3 GetMovementInputDirection()
        {
            return new Vector3(_playerMovementStateMachine.ReusableData.MovementInput.x, 0, _playerMovementStateMachine.ReusableData.MovementInput.y);
        }
        
        protected virtual void AddInputActionCallbacks()
        {
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
        }

        protected virtual void RemoveInputActionCallbacks()
        {
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
        }
        
        protected float GetMovementSpeed()
        {
            //Debug.Log(_playerMovementStateMachine.ReusableData.MovementOnSlopesSpeedModifier);
            
            return _playerGroundedData.BaseSpeed 
                   * _playerMovementStateMachine.ReusableData.MovementSpeedModifier 
                   * _playerMovementStateMachine.ReusableData.MovementOnSlopesSpeedModifier;
        }
        
        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 currentPlayerVelocity = _playerMovementStateMachine.Player.Rigidbody.velocity;
            
            currentPlayerVelocity.y = 0;
            
            return currentPlayerVelocity;
        }
        
        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, _playerMovementStateMachine.Player.Rigidbody.velocity.y, 0f);
        }
        
        protected void SetBaseRotationData()
        {
            _playerMovementStateMachine.ReusableData.RotationData = _playerGroundedData.RotationData;

            _playerMovementStateMachine.ReusableData.TimeToReachTargetRotation =
                _playerGroundedData.RotationData.TargetRotationReachTime;
        }

        protected void  RotateTowardsTargetRotation()
        {
            float currentYAngle = _playerMovementStateMachine.Player.Rigidbody.rotation.eulerAngles.y;
            
            if(currentYAngle == _playerMovementStateMachine.ReusableData.CurrentTargetRotation.y) return;
            
            float smoothYAngle = Mathf.SmoothDampAngle(
                currentYAngle, 
                _playerMovementStateMachine.ReusableData.CurrentTargetRotation.y, 
                ref _playerMovementStateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, 
                _playerMovementStateMachine.ReusableData.TimeToReachTargetRotation.y - _playerMovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y);
            _playerMovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;
            
            Quaternion targetRotation = Quaternion.Euler(0, smoothYAngle, 0);
            _playerMovementStateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }
        
        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);

            if(shouldConsiderCameraRotation)
                directionAngle = AddCameraRotationToAngle(directionAngle);

            if (directionAngle != _playerMovementStateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            return directionAngle;
        }
        
        protected Vector3 GetTargetRotationDirection(float targetAngle)
        {
            Vector3 targetRotationDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            
            return targetRotationDirection;
        }
        
        protected void ResetVelocity()
        {
            _playerMovementStateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        protected void ResetVerticalVelocity()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
            _playerMovementStateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
        }

        protected void DecelerateHorizontally()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            _playerMovementStateMachine.Player.Rigidbody.AddForce(-playerHorizontalVelocity * _playerMovementStateMachine.ReusableData.MovementDecelerationForce,
                ForceMode.Acceleration);
        }
        
        protected void DecelerateVertically()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

            _playerMovementStateMachine.Player.Rigidbody.AddForce(-playerVerticalVelocity * _playerMovementStateMachine.ReusableData.MovementDecelerationForce,
                ForceMode.Acceleration);
        }

        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
            
            Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);
            
            return playerHorizontalMovement.magnitude > minimumMagnitude;
        }

        protected bool IsMovingUp(float minimumVelocity = 0.1f)
        {
            return GetPlayerVerticalVelocity().y > minimumVelocity;
        }
        
        protected bool IsMovingDown(float minimumVelocity = 0.1f)
        {
            return GetPlayerVerticalVelocity().y < -minimumVelocity;
        }
        
        protected virtual void OnContactWithGround(Collider colliders)
        {
            
        }
        
        protected virtual void OnContactWithGroundExited(Collider colliders)
        {
            
        }
        
        #endregion

        #region Input Methods

        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            _playerMovementStateMachine.ReusableData.ShouldWalk = !_playerMovementStateMachine.ReusableData.ShouldWalk;
        }

        #endregion
    }
}
