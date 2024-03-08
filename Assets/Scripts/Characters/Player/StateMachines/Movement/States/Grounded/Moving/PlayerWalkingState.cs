using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerWalkingState : PlayerMovingState
    {
        private PlayerWalkData _playerWalkData;
        
        public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _playerWalkData = playerGroundedData.WalkData;
        }

        #region IState Methods

        public override void Enter()
        {
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = playerGroundedData.WalkData.SpeedModifier;
            
            playerMovementStateMachine.ReusableData.BackwardsCameraRecenteringData = _playerWalkData.BackwardsCameraRecenteringData;
            
            base.Enter();
            
            StartAnimation(playerMovementStateMachine.Player.AnimationData.WalkParameterHash);
            
            playerMovementStateMachine.ReusableData.CurrentJumpForce = playerAirborneData.JumpData.WeakForce;
        }

        public override void Exit()
        {
            base.Exit();
            
            StopAnimation(playerMovementStateMachine.Player.AnimationData.WalkParameterHash);
            
            SetBaseCameraRecenteringData();
        }

        #endregion

        #region Input Methods

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.LightStoppingState);
            
            base.OnMovementCanceled(context);
        }

        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);
            
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.RunningState);
        }

        #endregion
    }
}
