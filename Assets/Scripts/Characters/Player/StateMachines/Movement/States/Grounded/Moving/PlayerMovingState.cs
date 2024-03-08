using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerMovingState : PlayerGroundedState
    {
        public PlayerMovingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        
        #region IState Methods
        
public override void Enter()
        {
            base.Enter();
            
            StartAnimation(playerMovementStateMachine.Player.AnimationData.MovingParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            
            StopAnimation(playerMovementStateMachine.Player.AnimationData.MovingParameterHash);
        }
        
        #endregion
        
    }
}
