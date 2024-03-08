using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerStoppingState : PlayerGroundedState
    {
        public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods

        public override void Enter()
        {
            playerMovementStateMachine.ReusableData.MovementSpeedModifier = 0f;
            
            SetBaseCameraRecenteringData();
            
            base.Enter();
            
            StartAnimation(playerMovementStateMachine.Player.AnimationData.StoppingParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            
            StopAnimation(playerMovementStateMachine.Player.AnimationData.StoppingParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            RotateTowardsTargetRotation();
            
            if(!IsMovingHorizontally()) return;
            
            DecelerateHorizontally();
        }

        public override void OnAnimationTransitionEvent()
        {
            playerMovementStateMachine.ChangeState(playerMovementStateMachine.IdlingState);
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

        #endregion

        #region Input Methods
        
        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            OnMove();
        }

        #endregion
    }
}
