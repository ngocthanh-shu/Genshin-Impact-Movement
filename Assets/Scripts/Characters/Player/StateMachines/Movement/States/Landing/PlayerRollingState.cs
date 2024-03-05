using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerRollingState : PlayerLandingState
    {
        private PlayerRollData _playerRollData;
        public PlayerRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _playerRollData = _playerGroundedData.RollData;
        }

        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            
            _playerMovementStateMachine.ReusableData.MovementSpeedModifier = _playerRollData.SpeedModifier;
            
            _playerMovementStateMachine.ReusableData.ShouldSprint = false;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            if(_playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero) return;
            
            RotateTowardsTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            base.OnAnimationTransitionEvent();

            if (_playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.MediumStoppingState);
                
                return;
            }
            OnMove();
        }

        #endregion

        #region Input Methods

        protected override void OnJumpStarted(InputAction.CallbackContext obj)
        {
            
        }

        #endregion
    }
}
