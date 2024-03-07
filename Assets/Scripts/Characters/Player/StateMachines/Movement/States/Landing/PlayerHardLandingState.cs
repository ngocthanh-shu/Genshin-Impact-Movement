using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerHardLandingState : PlayerLandingState
    {
        public PlayerHardLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods

        public override void Enter()
        {
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            
            base.Enter();
            
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.Disable();

            ResetVelocity();
        }
        
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if(!IsMovingHorizontally()) return;
            ResetVelocity();
        }
        
        public override void Exit()
        {
            base.Exit();
            
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.Enable();
        }

        public override void OnAnimationTransitionEvent()
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.IdlingState);
        }

        public override void OnAnimationExitEvent()
        {
            base.OnAnimationExitEvent();
            
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.Enable();
        }

        #endregion

        #region Resuable Methods

        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();
            
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.started += OnMovementStarted;
        }
        

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();
            
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.started -= OnMovementStarted;
        }

        protected override void OnMove()
        {
            if(playerMovementStateMachine.ReusableData.ShouldWalk) return;
            
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.RunningState);
        }

        #endregion

        #region Input Methods

        protected override void OnJumpStarted(InputAction.CallbackContext obj)
        {
            
        }

        private void OnMovementStarted(InputAction.CallbackContext obj)
        {
            OnMove();
        }

        #endregion
    }
}
