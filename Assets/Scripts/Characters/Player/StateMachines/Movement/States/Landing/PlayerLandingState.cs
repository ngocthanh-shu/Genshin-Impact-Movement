using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerLandingState : PlayerGroundedState
    {
        public PlayerLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        
        #region IState Methods
        
        public override void Enter()
        {
            base.Enter();
            
            StartAnimation(playerMovementStateMachine.Player.AnimationData.LandingParameterHash);
        }
        
        public override void Exit()
        {
            base.Exit();
            
            StopAnimation(playerMovementStateMachine.Player.AnimationData.LandingParameterHash);
        }
        
        #endregion

    }
}
