using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerLightLandingState : PlayerLandingState
    {
        public PlayerLightLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods

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

        public override void OnAnimationTransitionEvent()
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.IdlingState);
        }

        #endregion
        
    }
}
