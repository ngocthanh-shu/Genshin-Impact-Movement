using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerRunningState : PlayerMovingState
    {
        private float _startTime;
        private PlayerSprintData _playerSprintData;
        public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _playerSprintData = playerGroundedData.SprintData;
        }

        #region IState Methods

        public override void Enter()
        {
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = playerGroundedData.RunData.SpeedModifier;
            
            base.Enter();
            
            playerMovementStateMachine.ReusableData.CurrentJumpForce = playerAirborneData.JumpData.MediumForce;
            
            _startTime = Time.time;
        }

        public override void Update()
        {
            base.Update();
            if(!playerMovementStateMachine.ReusableData.ShouldWalk) return;
            if(_startTime < Time.time + _playerSprintData.RunToWalkTime) return;
            StopRunning();
        }

        #endregion
        
        #region Main Methods
        
        private void StopRunning()
        {
            if(playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                playerMovementStateMachine.ChangeState(playerMovementStateMachine.IdlingState);
                return;
            }
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.WalkingState);
        }
        
        #endregion

        #region Input Methods

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.MediumStoppingState);
            
            base.OnMovementCanceled(context);
        }

        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);
            
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.WalkingState);
        }

        #endregion
    }
}
