using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerAirborneState : PlayerMovementState
    {
        public PlayerAirborneState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            StartAnimation(playerMovementStateMachine.Player.AnimationData.AirborneParameterHash);
            ResetSprintState();
        }
        
        public override void Exit()
        {
            base.Exit();
            StopAnimation(playerMovementStateMachine.Player.AnimationData.AirborneParameterHash);
        }

        #endregion

        #region Reusable Methods

        protected override void OnContactWithGround(Collider colliders)
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.LightLandingState);
        }

        protected virtual void ResetSprintState()
        {
            playerMovementStateMachine.ReusableData.ShouldSprint = false;
        }

        #endregion
    }
}
