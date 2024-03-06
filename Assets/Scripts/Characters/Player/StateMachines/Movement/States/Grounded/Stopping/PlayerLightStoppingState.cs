using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerLightStoppingState : PlayerStoppingState
    {
        public PlayerLightStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            
            playerMovementStateMachine.ReusableData.MovementDecelerationForce = playerGroundedData.StopData.LightDecelerationForce;
            playerMovementStateMachine.ReusableData.CurrentJumpForce = playerAirborneData.JumpData.WeakForce;
        }

        #endregion
        
    }
}
