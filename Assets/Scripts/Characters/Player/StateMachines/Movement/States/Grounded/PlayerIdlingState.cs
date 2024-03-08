using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerIdlingState : PlayerGroundedState
    {
        private PlayerIdleData _playerIdleData;
        
        public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _playerIdleData = playerGroundedData.IdleData;
        }

        #region IState Method

        public override void Enter()
        {
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            
            playerMovementStateMachine.ReusableData.BackwardsCameraRecenteringData = _playerIdleData.BackwardsCameraRecenteringData;
            
            base.Enter();
            
            StartAnimation(playerMovementStateMachine.Player.AnimationData.IdleParameterHash);

            playerMovementStateMachine.ReusableData.CurrentJumpForce = playerAirborneData.JumpData.StationaryForce;
            
            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();
            
            StopAnimation(playerMovementStateMachine.Player.AnimationData.IdleParameterHash);
        }

        public override void Update()
        {
            base.Update();
            if(playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero) return;
            OnMove();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if(!IsMovingHorizontally()) return;
            ResetVelocity();
        }

        #endregion
        
    }
}
