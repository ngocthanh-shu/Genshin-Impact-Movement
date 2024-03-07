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
            _slopeData = base.playerMovementStateMachine.Player.CapsuleColliderUtility.SlopeData;
        }
        
        #region IState Method

        public override void Enter()
        {
            base.Enter();
            
            UpdateShouldSprintState();
            
            UpdateCameraCenteringState(playerMovementStateMachine.ReusableData.MovementInput);
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
            BoxCollider groundCheckCollider = playerMovementStateMachine.Player.CapsuleColliderUtility.PlayerTriggerColliderData.GroundCheckCollider;
            
            Vector3 groundColliderCenterInWorldSpace = groundCheckCollider.bounds.center;
            
            Collider[] overlapGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, 
                playerMovementStateMachine.Player.CapsuleColliderUtility.PlayerTriggerColliderData.GroundCheckColliderExtents, groundCheckCollider.transform.rotation, 
                playerMovementStateMachine.Player.LayerData.GroundLayerMask, 
                QueryTriggerInteraction.Ignore);
            
            return overlapGroundColliders.Length > 0;
        }
        
        private void UpdateShouldSprintState()
        {
            if (!playerMovementStateMachine.ReusableData.ShouldSprint) return;
            if(playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero) return;
            playerMovementStateMachine.ReusableData.ShouldSprint = false;
        }
        
        private void Float()
        {
            Vector3 capsuleColliderCenterInWorldSpace =
                playerMovementStateMachine.Player.CapsuleColliderUtility.CapsuleColliderData.Collider.bounds.center;
            Ray downWardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);
            
            if(Physics.Raycast(downWardsRayFromCapsuleCenter, 
                                out RaycastHit hit, 
                                _slopeData.FloatRayDistance, 
                                playerMovementStateMachine.Player.LayerData.GroundLayerMask,
                                QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downWardsRayFromCapsuleCenter.direction);
                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);
                if(slopeSpeedModifier == 0) return;

                float distanceToFloatingPoint = playerMovementStateMachine.Player
                                                    .CapsuleColliderUtility.CapsuleColliderData
                                                    .ColliderCenterInLocalSpace.y * playerMovementStateMachine.Player
                                                    .transform.localScale.y - hit.distance;
                if(distanceToFloatingPoint == 0) return;
                
                float amountToLift = distanceToFloatingPoint * _slopeData.StepReachForce - GetPlayerVerticalVelocity().y;
                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
                playerMovementStateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
            
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = playerGroundedData.SlopeSpeedAngles.Evaluate(angle);

            if (playerMovementStateMachine.ReusableData.MovementOnSlopesSpeedModifier != slopeSpeedModifier)
            {
                playerMovementStateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;
                
                UpdateCameraCenteringState(playerMovementStateMachine.ReusableData.MovementInput);
            }
            

            return slopeSpeedModifier;
        }

        #endregion
        
        #region Reusable Methods

        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();

            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Dash.started += OnDashStarted;
            
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();

            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Dash.started -= OnDashStarted;
            
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Jump.started -= OnJumpStarted;
        }

        protected virtual void OnMove()
        {
            if (playerMovementStateMachine.ReusableData.ShouldSprint)
            {
                playerMovementStateMachine.ChangeState(playerMovementStateMachine.SprintingState);
                return;
            }
            
            if (playerMovementStateMachine.ReusableData.ShouldWalk)
            {
                playerMovementStateMachine.ChangeState(playerMovementStateMachine.WalkingState);
                return;
            }
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.RunningState);
        }

        protected override void OnContactWithGroundExited(Collider colliders)
        {
            base.OnContactWithGroundExited(colliders);

            if(IsThereGroundUnderneath()) return;
            
            Vector3 capsuleColliderCenterInWorldSpace =
                playerMovementStateMachine.Player.CapsuleColliderUtility.CapsuleColliderData.Collider.bounds.center;
            
            Ray downWardsRayFromCapsuleBottom = new Ray(
                capsuleColliderCenterInWorldSpace - playerMovementStateMachine.Player.CapsuleColliderUtility.CapsuleColliderData.ColliderVerticalExtents, 
                Vector3.down);

            if (!Physics.Raycast(downWardsRayFromCapsuleBottom, out _, playerGroundedData.GroundToFallRayDistance,
                    playerMovementStateMachine.Player.LayerData.GroundLayerMask, QueryTriggerInteraction.Ignore))
            {
                OnFall();
            }
        }
        
        protected virtual void OnFall()
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.FallingState);
        }

        #endregion
        
        #region Input Methods
        
        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.DashingState);
        }
        
        protected virtual void OnJumpStarted(InputAction.CallbackContext obj)
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.JumpingState);
        }

        #endregion
    }
}
