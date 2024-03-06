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
            _playerRollData = playerGroundedData.RollData;
        }

        #region IState Methods

        public override void Enter()
        {
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = _playerRollData.SpeedModifier;
            
            base.Enter();
            
            playerMovementStateMachine.ReusableData.ShouldSprint = false;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            if(playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero) return;
            
            RotateTowardsTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            base.OnAnimationTransitionEvent();

            if (playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                playerMovementStateMachine.ChangeState(playerMovementStateMachine.MediumStoppingState);
                
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
