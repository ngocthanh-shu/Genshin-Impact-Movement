using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    [Serializable]
    public class PlayerCameraUtility
    {
        [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
        [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0f;
        [field: SerializeField] public float DefaultHorizontalRecenteringTime { get; private set; } = 4f;
        
        private CinemachinePOV _pov;

        public void Initialize()
        {
            _pov = VirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        }

        public void EnableRecentering(float waitTime = -1f, float recenteringTime = -1f, float baseMovementSpeed = 1f,
            float movementSpeed = 1f)
        {
            _pov.m_HorizontalRecentering.m_enabled = true;
            
            _pov.m_HorizontalRecentering.CancelRecentering();

            if (waitTime == -1)
            {
                waitTime = DefaultHorizontalWaitTime;
            }
            
            if (recenteringTime == -1)
            {
                recenteringTime = DefaultHorizontalRecenteringTime;
            }
            
            recenteringTime *= baseMovementSpeed/movementSpeed;
            
            _pov.m_HorizontalRecentering.m_WaitTime = waitTime;
            _pov.m_HorizontalRecentering.m_RecenteringTime = recenteringTime;
        }

        public void DisableRecentering()
        {
            _pov.m_HorizontalRecentering.m_enabled = false;
        }
    }
}
