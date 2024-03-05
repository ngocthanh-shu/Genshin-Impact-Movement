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
            
            _playerMovementStateMachine.ReusableData.MovementDecelerationForce = _playerGroundedData.StopData.MediumDecelerationForce;
            _playerMovementStateMachine.ReusableData.CurrentJumpForce = _playerAirborneData.JumpData.MediumForce;
        }

        #endregion
    }
}
