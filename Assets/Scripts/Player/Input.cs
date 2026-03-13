using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tractorio.Player
{
    [RequireComponent(typeof(PrometeoCarController))]
    public sealed class Input : NetworkBehaviour
    {
        [SerializeField]
        private InputActionReference move;
        [SerializeField]
        private InputActionReference jump;
        
        private PrometeoCarController carController;
        
        private bool deceleratingCar;

        private void Awake()
        {
            carController = GetComponent<PrometeoCarController>();
        }

        private void Update()
        {
            if (!IsLocalPlayer) return;
            
            UpdateRpc(move.action.ReadValue<Vector2>(), jump.action.IsPressed(), jump.action.WasReleasedThisFrame());
        }
        
        

        [Rpc(SendTo.Server)]
        private void UpdateRpc(Vector2 movement, bool handbrakePressed, bool handbrakeJustReleased)
        {
            switch (movement.y)
            {
                case > 0:
                    carController.CancelInvoke(nameof(PrometeoCarController.DecelerateCar));
                    deceleratingCar = false;
                    carController.GoForward();
                    break;
                case < 0:
                    carController.CancelInvoke(nameof(PrometeoCarController.DecelerateCar));
                    deceleratingCar = false;
                    carController.GoReverse();
                    break;
            }

            if(movement.x < 0){
                carController.TurnLeft();
            }
            if(movement.x > 0){
                carController.TurnRight();
            }
            if(handbrakePressed){
                carController.CancelInvoke(nameof(PrometeoCarController.DecelerateCar));
                deceleratingCar = false;
                carController.Handbrake();
            }
            if(handbrakeJustReleased){
                carController.RecoverTraction();
            }
            if(Mathf.Approximately(movement.y, 0)){
                carController.ThrottleOff();
            }
            if(Mathf.Approximately(movement.y, 0) && !handbrakePressed && !deceleratingCar){
                carController.InvokeRepeating(nameof(PrometeoCarController.DecelerateCar), 0f, 0.1f);
                deceleratingCar = true;
            }
            if(Mathf.Approximately(movement.x, 0) && carController.SteeringAxes != 0f){
                carController.ResetSteeringAngle();
            }
        }
    }
}