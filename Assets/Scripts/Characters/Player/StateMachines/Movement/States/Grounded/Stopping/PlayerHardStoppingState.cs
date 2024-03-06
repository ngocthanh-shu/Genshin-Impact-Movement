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
            
            playerMovementStateMachine.ReusableData.MovementDecelerationForce = playerGroundedData.StopData.HardDecelerationForce;
            playerMovementStateMachine.ReusableData.CurrentJumpForce = playerAirborneData.JumpData.StrongForce;
        }

        #endregion

        #region Resuable Methods

        protected override void OnMove()
        {
            if(playerMovementStateMachine.ReusableData.ShouldWalk) return;
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.RunningState);
        }

        #endregion
    }
}
