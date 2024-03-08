using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerSprintingState : PlayerMovingState
    {
        private PlayerSprintData _playerSprintData;
        private bool _keepSprinting;
        private float _startSprintTime;

        private bool _shouldResetSprintState;
        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _playerSprintData = playerGroundedData.SprintData;
        }
        
        #region IState Methods

        public override void Enter()
        {
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = _playerSprintData.SpeedModifier;
            
            base.Enter();
            
            StartAnimation(playerMovementStateMachine.Player.AnimationData.SprintParameterHash);
            
            playerMovementStateMachine.ReusableData.CurrentJumpForce = playerAirborneData.JumpData.StrongForce;
            
            _shouldResetSprintState = true;
            
            _startSprintTime = Time.time;
        }

        public override void Update()
        {
            base.Update();
            if(_keepSprinting) return;
            if(Time.time < _startSprintTime + _playerSprintData.SprintToRunTime) return;
            StopSprinting();
        }

        public override void Exit()
        {
            base.Exit();
            
            StopAnimation(playerMovementStateMachine.Player.AnimationData.SprintParameterHash);

            if (_shouldResetSprintState)
            {
                _keepSprinting = false;
                playerMovementStateMachine.ReusableData.ShouldSprint = false;
            }
        }

        #endregion
        
        #region Main Methods
        
        private void StopSprinting()
        {
            if(playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                playerMovementStateMachine.ChangeState(playerMovementStateMachine.IdlingState);
                return;
            }
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.RunningState);
        }
        
        #endregion

        #region Reusuables Methods

        protected override void OnFall()
        {
            _keepSprinting = false;
            
            base.OnFall();
        }

        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Sprint.performed += OnSprintPerformed;
        }
        
        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Sprint.performed -= OnSprintPerformed;
        }

        #endregion
        
        #region Input Methods

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.HardStoppingState);
            
            base.OnMovementCanceled(context);
        }

        protected override void OnJumpStarted(InputAction.CallbackContext obj)
        {
            _shouldResetSprintState = false;
            base.OnJumpStarted(obj);
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _keepSprinting = true;
            
            playerMovementStateMachine.ReusableData.ShouldSprint = true;
        }
        
        #endregion
    }
}
