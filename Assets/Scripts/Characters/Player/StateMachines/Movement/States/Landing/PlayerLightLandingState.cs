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
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            
            base.Enter();

            playerMovementStateMachine.ReusableData.CurrentJumpForce = playerAirborneData.JumpData.StationaryForce;
            
            ResetVelocity();
        }

        public override void Update()
        {
            base.Update();
            
            if(playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero) return;
            
            OnMove();
        }

        public override void OnAnimationTransitionEvent()
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.IdlingState);
        }

        #endregion
        
    }
}
