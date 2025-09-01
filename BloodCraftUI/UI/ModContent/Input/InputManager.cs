using UnityEngine;

namespace BloodCraftEZLife.UI.ModContent.Input;

public static class InputManager
{
    public static MouseState Mouse = new MouseState();

    public static void Update()
    {
        Mouse.Position = UnityEngine.Input.mousePosition;
        Mouse.ScrollDelta = UnityEngine.Input.mouseScrollDelta;
        Mouse.Button0 = GetButtonState(0);
        Mouse.Button1 = GetButtonState(1);
        Mouse.Button2 = GetButtonState(2);
    }

    private static MouseState.ButtonState GetButtonState(int button)
    {
        if (UnityEngine.Input.GetMouseButtonUp(button))
        {
            return MouseState.ButtonState.Up | MouseState.ButtonState.Released;
        }
        else if (UnityEngine.Input.GetMouseButtonDown(button))
        {
            return MouseState.ButtonState.Down | MouseState.ButtonState.Clicked;
        }
        else if (UnityEngine.Input.GetMouseButton(button))
        {
            return MouseState.ButtonState.Down;
        }

        return MouseState.ButtonState.Up;
    }
}