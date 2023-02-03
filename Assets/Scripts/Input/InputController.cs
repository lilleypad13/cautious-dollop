using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputStrength
{
    Standard,
    Strong
}

public class InputController : MonoBehaviour
{
    // Emit an event with:
    // Position
    // Strength
    public static event Action<Vector2, InputStrength> CurrentMousePosition;



    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CurrentMousePosition?.Invoke(Input.mousePosition, InputStrength.Strong);
        }
        else
        {
            CurrentMousePosition?.Invoke(Input.mousePosition, InputStrength.Standard);
        }
    }
}
