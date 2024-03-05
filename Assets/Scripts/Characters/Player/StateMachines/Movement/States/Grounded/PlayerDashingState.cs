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
            _playerDashData = _playerGroundedData.DashData;
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
            base.Enter();
            _playerMovementStateMachine.ReusableData.MovementSpeedModifier = _playerDashData.SpeedModifier;
            _playerMovementStateMachine.ReusableData.RotationData = _playerDashData.RotationData;
            AddForceOnTransitionFromStationaryState();
            _shouldKeepRotating = _playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero;
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
            if (_playerMovementStateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.HardStoppingState);
                
                return;
            }
            _playerMovementStateMachine.ChangeState(_playerMovementStateMachine.SprintingState);
        }

        #endregion
        
        #region Main Methods
        
        private void AddForceOnTransitionFromStationaryState()
        {
            if(_playerMovementStateMachine.ReusableData.MovementInput != Vector2.zero) return;
            Vector3 characterRotationDirection = _playerMovementStateMachine.Player.transform.forward;
            characterRotationDirection.y = 0f;
            UpdateTargetRotation(characterRotationDirection, false);
            _playerMovementStateMachine.Player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();
        }
        
        private void UpdateConsecutiveDashes()
        {
            if (!IsConsecutive())
            {
                _consecutiveDashesUsed = 0;
            }

            ++_consecutiveDashesUsed;

            if (_consecutiveDashesUsed == _playerGroundedData.DashData.ConsecutiveDashesLimitAmount)
            {
                _consecutiveDashesUsed = 0;
                _playerMovementStateMachine.Player.PlayerInput.DisableActionFor(
                    _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Dash,
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
            
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.performed += OnMovementPerformed;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();
            _playerMovementStateMachine.Player.PlayerInput.PlayerActions.Movement.performed -= OnMovementPerformed;
        }
        
        #endregion

        #region Input Methods

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            
        }
        
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
