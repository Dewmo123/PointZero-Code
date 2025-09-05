using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Core.GameSystem
{
    public enum ScreenArea
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "SO/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject,Controls.IPlayerActions
    {
        [SerializeField] private LayerMask whatIsGround;

        public event Action<bool> OnAimEvent;
        public event Action<bool> OnPlantEvent;
        public event Action<bool> OnAttackEvent;
        public event Action<bool> OnCameraRotateEvent;
        public event Action<bool> OnSprintEvent;
        public event Action OnInteractEvent;
        public event Action OnReloadEvent;
        public event Action OnSettingEvent;
        private Controls _controls;
        public Vector2 MovementKey { get; private set; }
        public Vector2 MouseDelta { get; private set; }

        public Vector2 ScreenPosition;
        private Vector3 _worldPosition;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
        }
        private void OnDisable()
        {
            _controls.Player.Disable();
        }
        public void SetEnable(bool val)
        {
            if (val)
                _controls.Player.Enable();
            else
                _controls.Player.Disable();

        }
        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnAimEvent?.Invoke(true);
                OnCameraRotateEvent?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnCameraRotateEvent?.Invoke(false);
                OnAimEvent?.Invoke(false);
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAttackEvent?.Invoke(true);
            else if (context.canceled)
                OnAttackEvent?.Invoke(false);
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnInteractEvent?.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementKey = context.ReadValue<Vector2>();
        }

        public void OnPointer(InputAction.CallbackContext context)
        {
            ScreenPosition = context.ReadValue<Vector2>();
        }
        public Vector3 GetWorldPosition()
        {
            Camera mainCam = Camera.main;
            Debug.Assert(mainCam != null, "No main Cam in Scene");
            Ray camKey = mainCam.ScreenPointToRay(ScreenPosition);
            if (Physics.Raycast(camKey, out RaycastHit hit, mainCam.farClipPlane, whatIsGround))
            {
                _worldPosition = hit.point;
            }
            return _worldPosition;
        }
        public Ray GetCameraRay()=> Camera.main.ScreenPointToRay(ScreenPosition);
        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnSprintEvent?.Invoke(true);
            else if (context.canceled)
                OnSprintEvent?.Invoke(false);
        }

        public void OnPlant(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnPlantEvent?.Invoke(true);
            else if (context.canceled)
                OnPlantEvent?.Invoke(false);
        }

        public void OnMouseDelta(InputAction.CallbackContext context)
        {
            MouseDelta = context.ReadValue<Vector2>();
        }
        public void OnCameraRotate(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnCameraRotateEvent?.Invoke(true);
            else if (context.canceled)
                OnCameraRotateEvent?.Invoke(false);
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnReloadEvent?.Invoke();
        }

        public void OnSetting(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnSettingEvent?.Invoke();
        }
    }
}
