using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        
        private PlayerMovementStateMachine _movementStateMachine;

        [field:Header("References")]
        [field:SerializeField] public PlayerSO Data { get; private set; }
        
        [field:Header("Collisions")]
        [field:SerializeField] public PlayerCapsuleColliderUtility CapsuleColliderUtility { get; private set; }
        [field:SerializeField] public PlayerLayerData LayerData { get; private set; }
        
        [field: Header("Cameras")]
        [field:SerializeField] public PlayerCameraUtility CameraUtility { get; private set; }
        
        [field:Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }
        
        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public Transform CameraTransform { get; private set; }
        
        public PlayerInput PlayerInput { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>();
            
            if (Camera.main != null) CameraTransform = Camera.main.transform;

            PlayerInput = GetComponent<PlayerInput>();
            
            _movementStateMachine = new PlayerMovementStateMachine(this);
            
            CapsuleColliderUtility.Initialize(gameObject);
            CapsuleColliderUtility.CalculateCapsuleColliderDimensions();
            
            CameraUtility.Initialize();
            
            AnimationData.Initialize();
        }

        private void OnValidate()
        {
            CapsuleColliderUtility.Initialize(gameObject);
            CapsuleColliderUtility.CalculateCapsuleColliderDimensions();
        }

        private void Start()
        {
            _movementStateMachine.ChangeState(_movementStateMachine.IdlingState);
        }

        private void OnTriggerEnter(Collider other)
        {
            _movementStateMachine.OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            _movementStateMachine.OnTriggerExit(other);
        }

        private void Update()
        {
            _movementStateMachine.HandleInput();
            _movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            _movementStateMachine.PhysicsUpdate();
        }

        public void OnMovementStateAnimationEnterEvent()
        {
            _movementStateMachine.OnAnimationEnterEvent();
        }
        
        public void OnMovementStateAnimationExitEvent()
        {
            _movementStateMachine.OnAnimationExitEvent();
        }
        
        public void OnMovementStateAnimationTransitionEvent()
        {
            _movementStateMachine.OnAnimationTransitionEvent();
        }
    }
}
