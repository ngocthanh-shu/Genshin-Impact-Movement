using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerAnimationEventTrigger : MonoBehaviour
    {
        private Player _player;
        
        private void Awake()
        {
            _player = transform.parent.GetComponent<Player>();
        }
        
        public void TriggerOnMovementStateAnimationEnterEvent()
        {
            if(IsInAnimationTransition()) return;
            
            _player.OnMovementStateAnimationEnterEvent();
        }
        
        public void TriggerOnMovementStateAnimationExitEvent()
        {
            if(IsInAnimationTransition()) return;
            
            _player.OnMovementStateAnimationExitEvent();
        }
        
        public void TriggerOnMovementStateAnimationTransitionEvent()
        {
            if(IsInAnimationTransition()) return;
            
            _player.OnMovementStateAnimationTransitionEvent();
        }
        
        private bool IsInAnimationTransition(int layerIndex = 0)
        {
            return _player.Animator.IsInTransition(layerIndex);
        }
    }
}
