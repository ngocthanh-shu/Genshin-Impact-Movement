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
            base.Enter();
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.Disable();
            _playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            
            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.Enable();
        }

        public override void OnAnimationTransitionEvent()
        {
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.IdlingState);
        }

        public override void OnAnimationExitEvent()
        {
            base.OnAnimationExitEvent();
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.Enable();
        }

        #endregion

        #region Resuable Methods

        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.started += OnMovementStarted;
        }
        

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.started -= OnMovementStarted;
        }

        protected override void OnMove()
        {
            if(_playerMovementStateMachine.ReusableData.ShouldWalk) return;
            
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.RunningState);
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
