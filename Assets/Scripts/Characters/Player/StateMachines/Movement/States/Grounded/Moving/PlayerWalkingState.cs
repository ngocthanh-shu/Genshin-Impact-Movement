using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerWalkingState : PlayerMovingState
    {
        public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            
            _playerMovementStateMachine.ReusableData.MovementSpeedModifier = _playerGroundedData.WalkData.SpeedModifier;
            _playerMovementStateMachine.ReusableData.CurrentJumpForce = _playerAirborneData.JumpData.WeakForce;
        }

        #endregion

        #region Input Methods

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.LightStoppingState);
        }

        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);
            
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.RunningState);
        }

        #endregion
    }
}
