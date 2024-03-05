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
            ResetSprintState();
        }

        #endregion

        #region Reusable Methods

        protected override void OnContactWithGround(Collider colliders)
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.LightLandingState);
        }

        protected virtual void ResetSprintState()
        {
            _playerMovementStateMachine.ReusableData.ShouldSprint = false;
        }

        #endregion
    }
}
