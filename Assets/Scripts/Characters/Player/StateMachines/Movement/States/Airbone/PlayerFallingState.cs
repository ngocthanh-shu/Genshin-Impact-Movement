using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerFallingState : PlayerAirborneState
    {
        private PlayerFallData _playerFallData;
        
        private Vector3 _playerPositionOnEnter;
        public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _playerFallData = playerAirborneData.FallData;
        }

        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            
            StartAnimation(playerMovementStateMachine.Player.AnimationData.FallParameterHash);
            
            _playerPositionOnEnter = playerMovementStateMachine.Player.transform.position;

            playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            
            ResetVerticalVelocity();
        }
        
        public override void Exit()
        {
            base.Exit();
            
            StopAnimation(playerMovementStateMachine.Player.AnimationData.FallParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            LimitVerticalVelocity();
        }

        

        #endregion
        
        #region Reusable Methods

        protected override void ResetSprintState()
        {
            
        }

        protected override void OnContactWithGround(Collider colliders)
        {
            float fallDistance = _playerPositionOnEnter.y - playerMovementStateMachine.Player.transform.position.y;

            if (fallDistance < _playerFallData.MinimumDistanceToConsideredHardFall)
            {
                playerMovementStateMachine.ChangeState(playerMovementStateMachine.LightLandingState);
                
                return;
            }
            
            if(playerMovementStateMachine.ReusableData.ShouldWalk && !playerMovementStateMachine.ReusableData.ShouldSprint || playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                playerMovementStateMachine.ChangeState(playerMovementStateMachine.HardLandingState);
                
                return;
            }
            
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.RollingState);
        }

        #endregion

        #region Main Methods

        private void LimitVerticalVelocity()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
            if(playerVerticalVelocity.y >= -_playerFallData.FallSpeedLimit) return;
            
            Vector3 limitedVelocity = new Vector3(0f, -_playerFallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);
            
            playerMovementStateMachine.Player.Rigidbody.AddForce(limitedVelocity, ForceMode.VelocityChange);
        }

        #endregion
    }
}
