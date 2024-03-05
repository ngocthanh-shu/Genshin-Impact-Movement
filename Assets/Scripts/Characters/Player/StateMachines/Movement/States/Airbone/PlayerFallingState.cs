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
            _playerFallData = _playerAirborneData.FallData;
        }

        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            
            _playerPositionOnEnter = _playerMovementStateMachine.Player.transform.position;

            _playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            
            ResetVerticalVelocity();
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
            float fallDistance = Math.Abs(_playerPositionOnEnter.y - _playerMovementStateMachine.Player.transform.position.y);

            if (fallDistance < _playerFallData.MinimumDistanceToConsideredHardFall)
            {
                _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.LightLandingState);
                
                return;
            }
            
            if(_playerMovementStateMachine.ReusableData.ShouldWalk && !_playerMovementStateMachine.ReusableData.ShouldSprint || _playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.HardLandingState);
                
                return;
            }
            
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.RollingState);
        }

        #endregion

        #region Main Methods

        private void LimitVerticalVelocity()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
            if(playerVerticalVelocity.y >= -_playerFallData.FallSpeedLimit) return;
            
            Vector3 limitedVelocity = new Vector3(0f, -_playerFallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);
            
            _playerMovementStateMachine.Player.Rigidbody.AddForce(limitedVelocity, ForceMode.VelocityChange);
        }

        #endregion
    }
}
