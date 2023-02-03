using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseInput
{
    None,
    LeftClick, // While Held
    LeftClickDown
}

public class InputController : MonoBehaviour
{
    // Emit an event with:
    // Position
    // Strength
    public static event Action<Vector2, MouseInput> CurrentMousePosition;



    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentMousePosition?.Invoke(Input.mousePosition, MouseInput.LeftClickDown);
        }
        else if (Input.GetMouseButton(0))
        {
            CurrentMousePosition?.Invoke(Input.mousePosition, MouseInput.LeftClick);
        }
        else
        {
            CurrentMousePosition?.Invoke(Input.mousePosition, MouseInput.None);
        }
    }
}
