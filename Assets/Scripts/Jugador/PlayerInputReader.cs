using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-200)]
public class PlayerInputReader : MonoBehaviour
{
    public Vector2 Movement { get; private set; }
    public Vector2 Look { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool JumpPressed { get; private set; }

    public event Action FireRequested;
    public event Action PerspectiveToggleRequested;
    public event Action FlashlightToggleRequested;

    private void Update()
    {
        ResetFrameInput();

        if (Keyboard.current != null)
        {
            ReadKeyboard();
        }

        if (Mouse.current != null &&
            Cursor.lockState == CursorLockMode.Locked)
        {
            Look = Mouse.current.delta.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                FireRequested?.Invoke();
            }
        }
    }

    private void ReadKeyboard()
    {
        Keyboard keyboard = Keyboard.current;
        Vector2 movement = Vector2.zero;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            movement.y += 1f;

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            movement.y -= 1f;

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            movement.x += 1f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            movement.x -= 1f;

        Movement = Vector2.ClampMagnitude(movement, 1f);
        SprintHeld = keyboard.leftShiftKey.isPressed;
        JumpPressed = keyboard.spaceKey.wasPressedThisFrame;

        if (keyboard.tKey.wasPressedThisFrame)
        {
            PerspectiveToggleRequested?.Invoke();
        }

        if (keyboard.fKey.wasPressedThisFrame)
        {
            FlashlightToggleRequested?.Invoke();
        }
    }

    private void ResetFrameInput()
    {
        Movement = Vector2.zero;
        Look = Vector2.zero;
        SprintHeld = false;
        JumpPressed = false;
    }

    private void OnDisable()
    {
        ResetFrameInput();
    }
}