using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerMovementState : IState
    {
        protected PlayerMovementStateMachine playerMovementStateMachine;
        
        protected PlayerGroundedData playerGroundedData;
        protected PlayerAirborneData playerAirborneData;

        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            this.playerMovementStateMachine = playerMovementStateMachine;
            
            playerGroundedData = this.playerMovementStateMachine.Player.Data.GroundedData;
            playerAirborneData = this.playerMovementStateMachine.Player.Data.AirborneData;
            
            SetBaseCameraRecenteringData();

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
            if (playerMovementStateMachine.Player.LayerData.IsGroundLayer(colliders.gameObject.layer))
            {
                OnContactWithGround(colliders);
                
                return;
            }
        }

        public void OnTriggerExit(Collider colliders)
        {
            if (playerMovementStateMachine.Player.LayerData.IsGroundLayer(colliders.gameObject.layer))
            {
                OnContactWithGroundExited(colliders);
                
                return;
            }
        }

        #endregion

        #region Main Methods

        private void ReadMovementInput()
        {
            playerMovementStateMachine.ReusableData.MovementInput = playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.ReadValue<Vector2>();
        }

        private void Move()
        {
            if(playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero || playerMovementStateMachine.ReusableData.MovementSpeedModifier == 0) return;
            Vector3 movementDirection = GetMovementInputDirection();
            
            float targetRotationYAngle = Rotate(movementDirection);
            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float movementSpeed = GetMovementSpeed();
            
            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
            
            playerMovementStateMachine.Player.Rigidbody
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
            angle += playerMovementStateMachine.Player.CameraTransform.eulerAngles.y;
            
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
            playerMovementStateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;
            playerMovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0;
        }

        #endregion
        
        #region Reusable Methods
        
        protected void SetBaseCameraRecenteringData()
        {
            playerMovementStateMachine.ReusableData.BackwardsCameraRecenteringData =
                playerGroundedData.BackwardsCameraRecenteringData;
            playerMovementStateMachine.ReusableData.SidewaysCameraRecenteringData =
                playerGroundedData.SidewaysCameraRecenteringData;
        }
        
        protected Vector3 GetMovementInputDirection()
        {
            return new Vector3(playerMovementStateMachine.ReusableData.MovementInput.x, 0, playerMovementStateMachine.ReusableData.MovementInput.y);
        }
        
        protected virtual void AddInputActionCallbacks()
        {
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Look.started += OnMouseMovementStarted;
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.performed += OnMovementPerformed;
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.canceled += OnMovementCanceled;
        }

        protected virtual void RemoveInputActionCallbacks()
        {
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Look.started -= OnMouseMovementStarted;
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.performed -= OnMovementPerformed;
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.canceled -= OnMovementCanceled;
        }
        
        protected float GetMovementSpeed()
        {
            //Debug.Log(_playerMovementStateMachine.ReusableData.MovementOnSlopesSpeedModifier);
            
            return playerGroundedData.BaseSpeed 
                   * playerMovementStateMachine.ReusableData.MovementSpeedModifier 
                   * playerMovementStateMachine.ReusableData.MovementOnSlopesSpeedModifier;
        }
        
        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 currentPlayerVelocity = playerMovementStateMachine.Player.Rigidbody.velocity;
            
            currentPlayerVelocity.y = 0;
            
            return currentPlayerVelocity;
        }
        
        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, playerMovementStateMachine.Player.Rigidbody.velocity.y, 0f);
        }
        
        protected void SetBaseRotationData()
        {
            playerMovementStateMachine.ReusableData.RotationData = playerGroundedData.RotationData;

            playerMovementStateMachine.ReusableData.TimeToReachTargetRotation =
                playerGroundedData.RotationData.TargetRotationReachTime;
        }

        protected void  RotateTowardsTargetRotation()
        {
            float currentYAngle = playerMovementStateMachine.Player.Rigidbody.rotation.eulerAngles.y;
            
            if(currentYAngle == playerMovementStateMachine.ReusableData.CurrentTargetRotation.y) return;
            
            float smoothYAngle = Mathf.SmoothDampAngle(
                currentYAngle, 
                playerMovementStateMachine.ReusableData.CurrentTargetRotation.y, 
                ref playerMovementStateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, 
                playerMovementStateMachine.ReusableData.TimeToReachTargetRotation.y - playerMovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y);
            playerMovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;
            
            Quaternion targetRotation = Quaternion.Euler(0, smoothYAngle, 0);
            playerMovementStateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }
        
        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);

            if(shouldConsiderCameraRotation)
                directionAngle = AddCameraRotationToAngle(directionAngle);

            if (directionAngle != playerMovementStateMachine.ReusableData.CurrentTargetRotation.y)
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
            playerMovementStateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        protected void ResetVerticalVelocity()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
            playerMovementStateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
        }

        protected void DecelerateHorizontally()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            playerMovementStateMachine.Player.Rigidbody.AddForce(-playerHorizontalVelocity * playerMovementStateMachine.ReusableData.MovementDecelerationForce,
                ForceMode.Acceleration);
        }
        
        protected void DecelerateVertically()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

            playerMovementStateMachine.Player.Rigidbody.AddForce(-playerVerticalVelocity * playerMovementStateMachine.ReusableData.MovementDecelerationForce,
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

        protected void UpdateCameraCenteringState(Vector2 movementInput)
        {
            if(movementInput == Vector2.zero) return;
            
            if (movementInput == Vector2.up)
            {
                DisableCameraRecentering();
                return;
            }
            
            float cameraVerticalAngle = playerMovementStateMachine.Player.CameraTransform.eulerAngles.x;
            
            if (cameraVerticalAngle >= 270f)
            {
                cameraVerticalAngle -= 360f;
            }
            
            cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);

            if (movementInput == Vector2.down)
            {
                SetCameraRencenteringState(cameraVerticalAngle, playerMovementStateMachine.ReusableData
                    .BackwardsCameraRecenteringData);
                return;
            }
            
            SetCameraRencenteringState(cameraVerticalAngle, playerMovementStateMachine.ReusableData
                .SidewaysCameraRecenteringData);
        }

        protected void EnableCameraRecentering(float waitTime = -1f, float recenteringTime = -1f)
        {
            float movementSpeed = GetMovementSpeed();

            if (movementSpeed == 0f)
            {
                movementSpeed = playerGroundedData.BaseSpeed;
            }
            
            playerMovementStateMachine.Player.CameraUtility.EnableRecentering(waitTime, recenteringTime, playerGroundedData.BaseSpeed, movementSpeed);
        }
        
        protected void DisableCameraRecentering()
        {
            playerMovementStateMachine.Player.CameraUtility.DisableRecentering();
        }
        
        protected void SetCameraRencenteringState(float cameraVerticalAngle,
            List<PlayerCameraRecenteringData> cameraRecenteringData)
        {
            foreach (PlayerCameraRecenteringData playerCameraRecenteringData in cameraRecenteringData)
            {
                if (!playerCameraRecenteringData.IsWithinRange(cameraVerticalAngle)) continue;
                EnableCameraRecentering(playerCameraRecenteringData.WaitTime,
                    playerCameraRecenteringData.RecenteringTime);
                
                return;
            }

            DisableCameraRecentering();
        }
        
        #endregion

        #region Input Methods

        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            playerMovementStateMachine.ReusableData.ShouldWalk = !playerMovementStateMachine.ReusableData.ShouldWalk;
        }
        
        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            DisableCameraRecentering();
        }
        
        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            UpdateCameraCenteringState(context.ReadValue<Vector2>());
        }
        
        private void OnMouseMovementStarted(InputAction.CallbackContext context)
        {
            UpdateCameraCenteringState(playerMovementStateMachine.ReusableData.MovementInput);
        }

        #endregion
    }
}
