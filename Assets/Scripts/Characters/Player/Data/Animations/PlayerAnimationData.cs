using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    [Serializable]
    public class PlayerAnimationData
    {
        [field: Header("States Group Parameter Names")]
        [field: SerializeField] private string groundedParameterName = "IsGround";
        [field: SerializeField] private string movingParameterName = "IsMoving";
        [field: SerializeField] private string stoppingParameterName = "IsStopping";
        [field: SerializeField] private string landingParameterName = "IsLanding";
        [field: SerializeField] private string airborneParameterName = "IsAirborne";
        
        [field: Header("Grounded Parameter Names")]
        [field: SerializeField] private string idleParameterName = "IsIdling";
        [field: SerializeField] private string dashParameterName = "IsDashing";
        [field: SerializeField] private string walkParameterName = "IsWalking";
        [field: SerializeField] private string runParameterName = "IsRunning";
        [field: SerializeField] private string sprintParameterName = "IsSprinting";
        [field: SerializeField] private string mediumStopParameterName = "IsMediumStopping";
        [field: SerializeField] private string hardStopParameterName = "IsHardStopping";
        [field: SerializeField] private string hardLandParameterName = "IsHardLanding";
        [field: SerializeField] private string rollParameterName = "IsRolling";
        
        [field: Header("Airborne Parameter Names")]
        [field: SerializeField] private string fallParameterName = "IsFalling";
        
        public int GroundedParameterHash { get; private set; }
        public int MovingParameterHash { get; private set; }
        public int StoppingParameterHash { get; private set; }
        public int LandingParameterHash { get; private set; }
        public int AirborneParameterHash { get; private set; }
        
        public int IdleParameterHash { get; private set; }
        public int DashParameterHash { get; private set; }
        public int WalkParameterHash { get; private set; }
        public int RunParameterHash { get; private set; }
        public int SprintParameterHash { get; private set; }
        public int MediumStopParameterHash { get; private set; }
        public int HardStopParameterHash { get; private set; }
        public int HardLandParameterHash { get; private set; }
        public int RollParameterHash { get; private set; }
        
        public int FallParameterHash { get; private set; }

        public void Initialize()
        {
            GroundedParameterHash = Animator.StringToHash(groundedParameterName);
            MovingParameterHash = Animator.StringToHash(movingParameterName);
            StoppingParameterHash = Animator.StringToHash(stoppingParameterName);
            LandingParameterHash = Animator.StringToHash(landingParameterName);
            AirborneParameterHash = Animator.StringToHash(airborneParameterName);
            
            IdleParameterHash = Animator.StringToHash(idleParameterName);
            DashParameterHash = Animator.StringToHash(dashParameterName);
            WalkParameterHash = Animator.StringToHash(walkParameterName);
            RunParameterHash = Animator.StringToHash(runParameterName);
            SprintParameterHash = Animator.StringToHash(sprintParameterName);
            MediumStopParameterHash = Animator.StringToHash(mediumStopParameterName);
            HardStopParameterHash = Animator.StringToHash(hardStopParameterName);
            HardLandParameterHash = Animator.StringToHash(hardLandParameterName);
            RollParameterHash = Animator.StringToHash(rollParameterName);
            
            FallParameterHash = Animator.StringToHash(fallParameterName);
        }
    }
}
