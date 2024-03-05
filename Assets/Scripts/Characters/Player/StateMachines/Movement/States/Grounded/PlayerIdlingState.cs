using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerIdlingState : PlayerGroundedState
    {
        public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Method

        public override void Enter()
        {
            base.Enter();
            
            _playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            _playerMovementStateMachine.ReusableData.CurrentJumpForce = _playerAirborneData.JumpData.StationaryForce;
            
            ResetVelocity();
        }

        public override void Update()
        {
            base.Update();
            if(_playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero) return;
            OnMove();
        }

        #endregion
        
    }
}
