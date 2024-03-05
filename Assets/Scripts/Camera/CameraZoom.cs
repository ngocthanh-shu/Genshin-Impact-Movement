using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] [Range(0f,10f)] private float defaultDistance = 6f;
        [SerializeField] [Range(0f,10f)] private float minimumDistance = 1f;
        [SerializeField] [Range(0f,10f)] private float maximumDistance = 6f;
        
        [SerializeField] [Range(0f,10f)] private float smoothing = 4f;
        [SerializeField] [Range(0f,10f)] private float zoomSensitivity = 1f;
        
        private CinemachineFramingTransposer _framingTransposer;
        private CinemachineInputProvider _inputProvider;
        
        private float _currentTargetDistance;

        private void Awake()
        {
            _framingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
            _inputProvider = GetComponent<CinemachineInputProvider>();
            
            _currentTargetDistance = defaultDistance;
        }

        private void Update()
        {
            Zoom();
        }

        private void Zoom()
        {
            float zoomValue = _inputProvider.GetAxisValue(2) * zoomSensitivity * -1;
            _currentTargetDistance = Mathf.Clamp(_currentTargetDistance - zoomValue, minimumDistance, maximumDistance);
            
            float currentDistance = _framingTransposer.m_CameraDistance;
            
            if(currentDistance == _currentTargetDistance) return;
            
            float lerpedZoomValue = Mathf.Lerp(currentDistance, _currentTargetDistance, smoothing * Time.deltaTime);
            _framingTransposer.m_CameraDistance = lerpedZoomValue;
            
            
        }
    }
}
