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
            _playerSprintData = _playerGroundedData.SprintData;
        }

        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            
            _playerMovementStateMachine.ReusableData.MovementSpeedModifier = _playerGroundedData.RunData.SpeedModifier;
            _playerMovementStateMachine.ReusableData.CurrentJumpForce = _playerAirborneData.JumpData.MediumForce;
            
            _startTime = Time.time;
        }

        public override void Update()
        {
            base.Update();
            if(!_playerMovementStateMachine.ReusableData.ShouldWalk) return;
            if(_startTime < Time.time + _playerSprintData.RunToWalkTime) return;
            StopRunning();
        }

        #endregion
        
        #region Main Methods
        
        private void StopRunning()
        {
            if(_playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.IdlingState);
                return;
            }
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.WalkingState);
        }
        
        #endregion

        #region Input Methods

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.MediumStoppingState);
        }

        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);
            
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.WalkingState);
        }

        #endregion
    }
}
