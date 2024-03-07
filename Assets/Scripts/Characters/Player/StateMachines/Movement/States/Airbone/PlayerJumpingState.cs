using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerJumpingState : PlayerAirborneState
    {
        private bool _shouldKeepRotating;
        private bool _canStartFalling;

        private PlayerJumpData _jumpData;
        
        public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _jumpData = playerAirborneData.JumpData;
        }


        #region IState Methods

        public override void Enter()
        {
            base.Enter();

            playerMovementStateMachine.ReusableData.MovementDecelerationForce = _jumpData.DecelerationForce;
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            _shouldKeepRotating = playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero;
            
            Jump();
        }

        public override void Update()
        {
            base.Update();

            if (!_canStartFalling && IsMovingUp(0f))
            {
                _canStartFalling = true;
            }
            
            if(!_canStartFalling || GetPlayerVerticalVelocity().y > 0f) return;
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.FallingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_shouldKeepRotating)
            {
                RotateTowardsTargetRotation();
            }

            if (IsMovingUp())
            {
                DecelerateVertically();
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            SetBaseRotationData();
            
            _canStartFalling = false;
        }

        #endregion

        #region Main Methods

        private void Jump()
        {
            Vector3 jumpForce = playerMovementStateMachine.ReusableData.CurrentJumpForce;
            
            Vector3 jumpDirection = playerMovementStateMachine.Player.transform.forward;

            if (_shouldKeepRotating)
            {
                UpdateTargetRotation(GetMovementInputDirection());
                
                jumpDirection =
                    GetTargetRotationDirection(playerMovementStateMachine.ReusableData.CurrentTargetRotation.y);
            }
            
            jumpForce.x *= jumpDirection.x;
            jumpForce.z *= jumpDirection.z;

            Vector3 capsuleColliderCenterInWorldSpace = playerMovementStateMachine.Player.CapsuleColliderUtility
                .CapsuleColliderData.Collider.bounds.center;
            
            Ray downwardRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardRayFromCapsuleCenter, 
                    out RaycastHit hit, 
                    _jumpData.JumpToGroundRayDistance, 
                    playerMovementStateMachine.Player.LayerData.GroundLayerMask, 
                    QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardRayFromCapsuleCenter.direction);

                if (IsMovingUp())
                {
                    float jumpForceModifier = _jumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);
                    
                    jumpForce.x *= jumpForceModifier;
                    jumpForce.z *= jumpForceModifier;
                }

                if (IsMovingDown())
                {
                    float jumpForceModifier = _jumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);
                    
                    jumpForce.y *= jumpForceModifier;
                }
            }

            ResetVelocity();
            
            playerMovementStateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        #endregion

        #region Resuable Methods

        protected override void ResetSprintState()
        {
            
        }

        #endregion

        #region Input Methods

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
        }

        #endregion
        
    }
}
