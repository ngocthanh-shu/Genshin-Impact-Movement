using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerMediumStoppingState : PlayerStoppingState
    {
        public PlayerMediumStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        
        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            
            playerMovementStateMachine.ReusableData.MovementDecelerationForce = playerGroundedData.StopData.MediumDecelerationForce;
            playerMovementStateMachine.ReusableData.CurrentJumpForce = playerAirborneData.JumpData.MediumForce;
        }

        #endregion
    }
}
