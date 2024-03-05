using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerHardStoppingState : PlayerStoppingState
    {
        public PlayerHardStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        
        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            
            _playerMovementStateMachine.ReusableData.MovementDecelerationForce = _playerGroundedData.StopData.HardDecelerationForce;
            _playerMovementStateMachine.ReusableData.CurrentJumpForce = _playerAirborneData.JumpData.StrongForce;
        }

        #endregion

        #region Resuable Methods

        protected override void OnMove()
        {
            if(_playerMovementStateMachine.ReusableData.ShouldWalk) return;
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.RunningState);
        }

        #endregion
    }
}
