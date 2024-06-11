using System;
using UnityEngine;

namespace Manager
{
    public class InputManager : MonoBehaviour
    {
        private InputHandler _InputHandler;

        public static bool IsMainAction;
        public static bool IsSecondaryAction;

        private void Awake()
        {
            _InputHandler = new InputHandler();
        }
        
        private void OnEnable()
        {
            _InputHandler.Enable();
            
            _InputHandler.TouchWorld.MainAction.started += ctx => IsMainAction = !IsSecondaryAction;
            _InputHandler.TouchWorld.MainAction.canceled += ctx => IsMainAction = false;
            
            _InputHandler.TouchWorld.SecondaryAction.performed += ctx =>
            {
                IsSecondaryAction = true;
                IsMainAction = false;
            };
            _InputHandler.TouchWorld.SecondaryAction.canceled += ctx => IsSecondaryAction = false;
        }
        
        private void OnDisable()
        {
            _InputHandler.TouchWorld.MainAction.started -= ctx => IsMainAction = !IsSecondaryAction;
            _InputHandler.TouchWorld.MainAction.canceled -= ctx => IsMainAction = false;
            
            _InputHandler.TouchWorld.SecondaryAction.performed -= ctx =>
            {
                IsSecondaryAction = true;
                IsMainAction = false;
            };
            _InputHandler.TouchWorld.SecondaryAction.canceled -= ctx => IsSecondaryAction = false;
            
            _InputHandler.Disable();
        }
    }
}
