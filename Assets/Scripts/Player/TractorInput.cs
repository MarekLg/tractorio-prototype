using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public sealed class TractorInput : MonoBehaviour
    {
        private InputAction move;

        private void Awake()
        {
            move = InputSystem.actions.FindAction("Move");
        }

        private void Update()
        {
            var movement = move.ReadValue<Vector2>();

            if (movement.y > 0)
            {
                
            }
        }
    }
}