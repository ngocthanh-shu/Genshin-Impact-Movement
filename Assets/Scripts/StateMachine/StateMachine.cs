using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public abstract class StateMachine
    {
        protected IState currentState;

        public void ChangeState(IState state)
        {
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
        }
        
        public void HandleInput()
        {
            currentState.HandleInput();
        }
        
        public void Update()
        {
            currentState.Update();
        }
        
        public void PhysicsUpdate()
        {
            currentState.PhysicsUpdate();
        }
        
        public void OnAnimationEnterEvent()
        {
            currentState?.OnAnimationEnterEvent();
        }
        
        public void OnAnimationExitEvent()
        {
            currentState?.OnAnimationExitEvent();
        }
        
        public void OnAnimationTransitionEvent()
        {
            currentState?.OnAnimationTransitionEvent();
        }
        
        public void OnTriggerEnter(Collider colliders)
        {
            currentState?.OnTriggerEnter(colliders);
        }
        
        public void OnTriggerExit(Collider colliders)
        {
            currentState?.OnTriggerExit(colliders);
        }
    }
}
