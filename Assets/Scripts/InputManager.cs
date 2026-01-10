using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public static InputManager Instance { get; private set; }
    
    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2> OnLookInput;
    public event Action OnConfirmInput;
    public event Action<bool> OnSprintInput;

    private InputSystem_Actions _input;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _input = new InputSystem_Actions();
        _input.Player.AddCallbacks(this);
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
    private void OnDestroy() => _input.Dispose();

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveInput?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        OnLookInput?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnConfirmInput?.Invoke();
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSprintInput?.Invoke(true);
        }
        if (context.canceled)
        {
            OnSprintInput?.Invoke(false);
        }
    }
}
