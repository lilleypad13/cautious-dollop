using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInputReceiver : MonoBehaviour
{
    public Image sprite;
    public Vector3 defaultIndicatorScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 strongIndicatorScale = new Vector3(5.0f, 5.0f, 5.0f);
    public Camera targetCamera;

    public LineRenderer targetLine;
    public float currentLength;
    public float defaultSegmentLength = 3.0f;
    public float strongSegmentLength = 6.0f;


    private void Awake()
    {
        currentLength = defaultSegmentLength;
    }

    private void OnEnable()
    {
        InputController.CurrentMousePosition += DoSomething;
        InputController.CurrentMousePosition += RotateLineRenderer;
    }

    private void OnDisable()
    {
        InputController.CurrentMousePosition -= DoSomething;
        InputController.CurrentMousePosition -= RotateLineRenderer;
    }

    public void DoSomething(Vector2 vector, MouseInput strength)
    {
        sprite.transform.position = vector;
        Vector3 imageScale = defaultIndicatorScale;

        switch (strength)
        {
            case MouseInput.None:
                imageScale = defaultIndicatorScale;
                break;
            case MouseInput.LeftClick:
                imageScale = strongIndicatorScale;
                break;
            case MouseInput.LeftClickDown:
                Debug.Log("Just clicked mouse for icon");
                break;
            default:
                imageScale = defaultIndicatorScale;
                break;
        }

        sprite.transform.localScale = imageScale;
    }

    public void RotateLineRenderer(Vector2 vector, MouseInput strength)
    {
        // Add Line Renderer points
        Vector2 totalDirection = vector - (Vector2)targetCamera.WorldToScreenPoint(targetLine.GetPosition(0)); // probably needs to translate to mouse position first
        Vector2 direction = totalDirection.normalized;
        Vector2 lineTarget = (Vector2)targetLine.GetPosition(0) + direction * currentLength;

        targetLine.SetPosition(1, lineTarget);

        switch (strength)
        {
            case MouseInput.None:
                currentLength = defaultSegmentLength;
                break;
            case MouseInput.LeftClick:
                currentLength = strongSegmentLength;
                break;
            case MouseInput.LeftClickDown:
                Debug.Log("Just clicked mouse for line renderer");
                break;
            default:
                currentLength = defaultSegmentLength;
                break;
        }
    }
}
