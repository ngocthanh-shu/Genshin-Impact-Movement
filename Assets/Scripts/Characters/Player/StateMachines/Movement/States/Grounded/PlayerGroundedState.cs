using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerGroundedState : PlayerMovementState
    {
        private SlopeData _slopeData;
        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _slopeData = _playerMovementStateMachine.Player.CapsuleColliderUtility.SlopeData;
        }
        
        #region IState Method

        public override void Enter()
        {
            base.Enter();
            
            UpdateShouldSprintState();
        }
        
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            Float();
            
        }

        #endregion
        
        #region Main Methods
        
        private bool IsThereGroundUnderneath()
        {
            BoxCollider groundCheckCollider = _playerMovementStateMachine.Player.CapsuleColliderUtility.PlayerTriggerColliderData.GroundCheckCollider;
            
            Vector3 groundColliderCenterInWorldSpace = groundCheckCollider.bounds.center;
            
            Collider[] overlapGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, 
                groundCheckCollider.bounds.extents, groundCheckCollider.transform.rotation, 
                _playerMovementStateMachine.Player.LayerData.GroundLayerMask, 
                QueryTriggerInteraction.Ignore);
            
            return overlapGroundColliders.Length > 0;
        }
        
        private void UpdateShouldSprintState()
        {
            if (!_playerMovementStateMachine.ReusableData.ShouldSprint) return;
            if(_playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero) return;
            _playerMovementStateMachine.ReusableData.ShouldSprint = false;
        }
        
        private void Float()
        {
            Vector3 capsuleColliderCenterInWorldSpace =
                _playerMovementStateMachine.Player.CapsuleColliderUtility.CapsuleColliderData.Collider.bounds.center;
            Ray downWardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);
            
            if(Physics.Raycast(downWardsRayFromCapsuleCenter, 
                                out RaycastHit hit, 
                                _slopeData.FloatRayDistance, 
                                _playerMovementStateMachine.Player.LayerData.GroundLayerMask,
                                QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downWardsRayFromCapsuleCenter.direction);
                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);
                if(slopeSpeedModifier == 0) return;

                float distanceToFloatingPoint = _playerMovementStateMachine.Player
                                                    .CapsuleColliderUtility.CapsuleColliderData
                                                    .ColliderCenterInLocalSpace.y * _playerMovementStateMachine.Player
                                                    .transform.localScale.y - hit.distance;
                if(distanceToFloatingPoint == 0) return;
                
                float amountToLift = distanceToFloatingPoint * _slopeData.StepReachForce - GetPlayerVerticalVelocity().y;
                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
                _playerMovementStateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
            
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = _playerGroundedData.SlopeSpeedAngles.Evaluate(angle);
            _playerMovementStateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

            return slopeSpeedModifier;
        }

        #endregion
        
        #region Reusable Methods

        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.canceled += OnMovementCanceled;
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Dash.started += OnDashStarted;
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.canceled -= OnMovementCanceled;
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Dash.started -= OnDashStarted;
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Jump.started -= OnJumpStarted;
        }

        protected virtual void OnMove()
        {
            if (_playerMovementStateMachine.ReusableData.ShouldSprint)
            {
                _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.SprintingState);
                return;
            }
            
            if (_playerMovementStateMachine.ReusableData.ShouldWalk)
            {
                _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.WalkingState);
                return;
            }
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.RunningState);
        }

        protected override void OnContactWithGroundExited(Collider colliders)
        {
            base.OnContactWithGroundExited(colliders);

            if(IsThereGroundUnderneath()) return;
            
            Vector3 capsuleColliderCenterInWorldSpace =
                _playerMovementStateMachine.Player.CapsuleColliderUtility.CapsuleColliderData.Collider.bounds.center;
            
            Ray downWardsRayFromCapsuleBottom = new Ray(
                capsuleColliderCenterInWorldSpace - _playerMovementStateMachine.Player.CapsuleColliderUtility.CapsuleColliderData.ColliderVerticalExtents, 
                Vector3.down);

            if (!Physics.Raycast(downWardsRayFromCapsuleBottom, out _, _playerGroundedData.GroundToFallRayDistance,
                    _playerMovementStateMachine.Player.LayerData.GroundLayerMask, QueryTriggerInteraction.Ignore))
            {
                OnFall();
            }
        }
        
        protected virtual void OnFall()
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.FallingState);
        }

        #endregion
        
        #region Input Methods
        
        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.IdlingState);
        }
        
        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.DashingState);
        }
        
        protected virtual void OnJumpStarted(InputAction.CallbackContext obj)
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.JumpingState);
        }

        #endregion
    }
}
