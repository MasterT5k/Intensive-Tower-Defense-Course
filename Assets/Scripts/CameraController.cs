﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Other.CameraController
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 1f;
        [SerializeField]
        private Vector3 _startPos = Vector3.zero;
        [SerializeField]
        private float _xLimitMax = 0f;
        [SerializeField]
        private float _xLimitMin = 0f;
        private float _yPos;
        [SerializeField]
        private float _zLimitMax = 0f;
        [SerializeField]
        private float _zLimitMin = 0f;

        private Camera _myCamera;
        [SerializeField]
        private float _zoomOutLimit = 0f;
        [SerializeField]
        private float _zoomInLimit = 0f;

        [SerializeField]
        private bool _mousePan = false;
        [SerializeField]
        private float _screenEdgeOffset = 0f;
        private float _screenWidth;
        private float _screenHeight;

        void Start()
        {
            if (_startPos == Vector3.zero)
            {
                _startPos = transform.position;
            }
            transform.position = _startPos;
            _yPos = transform.position.y;
            _myCamera = Camera.main;
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;

            Cursor.lockState = CursorLockMode.Confined;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }

            CameraMovement();
        }

        void CameraMovement()
        {
            float hInput = Input.GetAxisRaw("Horizontal");
            float vInput = Input.GetAxisRaw("Vertical");
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;
            float scrollInput = Input.mouseScrollDelta.y;

            if (_mousePan == true)
            {
                if (mouseX < _screenEdgeOffset && hInput == 0)
                {
                    hInput = -1f;
                }
                else if (mouseX > (_screenWidth - _screenEdgeOffset) && hInput == 0)
                {
                    hInput = 1f;
                }

                if (mouseY < _screenEdgeOffset && vInput == 0)
                {
                    vInput = -1f;
                }
                else if (mouseY > (_screenHeight - _screenEdgeOffset) && vInput == 0)
                {
                    vInput = 1f;
                }
            }

            if (scrollInput != 0f)
            {
                _myCamera.fieldOfView += -scrollInput;
                _myCamera.fieldOfView = Mathf.Clamp(_myCamera.fieldOfView, _zoomInLimit, _zoomOutLimit);
            }

            if (hInput != 0f || vInput != 0f)
            {
                Vector3 direction = new Vector3(vInput, 0f, -hInput);
                Vector3 velocity = direction * _speed * Time.unscaledDeltaTime;
                Vector3 newPos = transform.position + velocity;
                newPos.y = _yPos;
                newPos.x = Mathf.Clamp(newPos.x, _xLimitMin, _xLimitMax);
                newPos.z = Mathf.Clamp(newPos.z, _zLimitMin, _zLimitMax);

                transform.position = newPos;
            }
        }
    }
}

