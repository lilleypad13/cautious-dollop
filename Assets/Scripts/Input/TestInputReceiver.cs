using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class TestInputReceiver : MonoBehaviour
{
    public Image sprite;
    public Vector3 defaultIndicatorScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 strongIndicatorScale = new Vector3(5.0f, 5.0f, 5.0f);
    public Camera targetCamera;

    public LineRenderer targetLine;
    public float segmentLength = 3.0f;

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

    public void DoSomething(Vector2 vector, InputStrength strength)
    {
        sprite.transform.position = vector;
        Vector3 imageScale = defaultIndicatorScale;

        switch (strength)
        {
            case InputStrength.Standard:
                imageScale = defaultIndicatorScale;
                break;
            case InputStrength.Strong:
                imageScale = strongIndicatorScale;
                break;
            default:
                imageScale = defaultIndicatorScale;
                break;
        }

        sprite.transform.localScale = imageScale;
    }

    public void RotateLineRenderer(Vector2 vector, InputStrength strength)
    {
        // Add Line Renderer points
        Vector2 totalDirection = vector - (Vector2)targetCamera.WorldToScreenPoint(targetLine.GetPosition(0)); // probably needs to translate to mouse position first
        Vector2 direction = totalDirection.normalized;
        Vector2 lineTarget = (Vector2)targetLine.GetPosition(0) + direction * segmentLength;

        targetLine.SetPosition(1, lineTarget);
    }
}
