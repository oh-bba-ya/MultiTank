using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;    // Controls 클래스 ( Action에서 설정한 경로에 존재하는 클래스)


[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<Vector2> MoveEvent;
    public event Action<bool> PrimaryFireEvenet;

    public Vector2 AimPosition { get; private set; }

    private Controls controls;

    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }

        controls.Player.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvenet?.Invoke(true);
        }
        else if (context.canceled)
        {
            PrimaryFireEvenet?.Invoke(false);
        }

    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }
}
