using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerDashingState : PlayerGroundedState
    {
        private PlayerDashData _playerDashData;
        private float _startDashTime;
        private int _consecutiveDashesUsed;
        
        private bool _shouldKeepRotating;
        
        //test
        //private float _dashToSprintTime = 1f;
        
        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            _playerDashData = playerGroundedData.DashData;
        }

        public override void Update()
        {
            base.Update();
            //if(Time.time < _startDashTime + _dashToSprintTime) return;
            //_playerMovementStateMachine.ChangeState(_playerMovementStateMachine.SprintingState);
            
        }
        
        #region IState Method

        public override void Enter()
        {
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = _playerDashData.SpeedModifier;
            base.Enter();
            playerMovementStateMachine.ReusableData.RotationData = _playerDashData.RotationData;
            Dash();
            _shouldKeepRotating = playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero;
            UpdateConsecutiveDashes();
            _startDashTime = Time.time;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if(!_shouldKeepRotating) return;
            RotateTowardsTargetRotation();
        }

        public override void Exit()
        {
            base.Exit();
            SetBaseRotationData();
        }

        public override void OnAnimationTransitionEvent()
        {
            if (playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                playerMovementStateMachine.ChangeState(playerMovementStateMachine.HardStoppingState);
                
                return;
            }
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.SprintingState);
        }

        #endregion
        
        #region Main Methods
        
        private void Dash()
        {
            Vector3 dashDirection = playerMovementStateMachine.Player.transform.forward;
            dashDirection.y = 0f;
            UpdateTargetRotation(dashDirection, false);

            if (playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                UpdateTargetRotation(GetMovementInputDirection());

                dashDirection =
                    GetTargetRotationDirection(playerMovementStateMachine.ReusableData.CurrentTargetRotation.y);
            }

            playerMovementStateMachine.Player.Rigidbody.velocity = dashDirection * GetMovementSpeed(false);
        }
        
        private void UpdateConsecutiveDashes()
        {
            if (!IsConsecutive())
            {
                _consecutiveDashesUsed = 0;
            }

            ++_consecutiveDashesUsed;

            if (_consecutiveDashesUsed == playerGroundedData.DashData.ConsecutiveDashesLimitAmount)
            {
                _consecutiveDashesUsed = 0;
                playerMovementStateMachine.Player.PlayerInput.DisableActionFor(
                    playerMovementStateMachine.Player.PlayerInput.PlayerActions.Dash,
                    _playerDashData.DashLimitReachCooldown);
                
            }
            
            
        }

        private bool IsConsecutive()
        {
            return Time.time < _startDashTime + _playerDashData.TimeToBeConsiderConsecutive;
        }

        #endregion

        #region Resuable Methods

        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();
            
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.performed += OnMovementPerformed;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();
            playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.performed -= OnMovementPerformed;
        }
        
        #endregion

        #region Input Methods
        
        private void OnMovementPerformed(InputAction.CallbackContext obj)
        {
            _shouldKeepRotating = true;
        }

        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
            
        }

        #endregion
    }
}
