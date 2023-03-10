using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraCtrl : MonoBehaviour
{
	public InputData data;

	private Vector3 tempPos;
	private bool isFocused = false;

	private SpriteRenderer spriteRenderer;

    void Awake()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

    private void OnEnable() {
		//Data stores radius, but scale is diameter
		transform.localScale = Vector3.one * 2 * data.standardInput.radius;
		InputController.CurrentMousePosition += GetMousePos;
	}

	private void OnDisable() {
		InputController.CurrentMousePosition -= GetMousePos;
	}

	private void GetMousePos(Vector2 mousePos, MouseInput inputType) {
		if(inputType == MouseInput.LeftClickDown) {
			isFocused = true;

			spriteRenderer.color = data.focusedInput.color;
		}
		else if (inputType == MouseInput.None && isFocused) {
			isFocused = false;

			spriteRenderer.color = data.standardInput.color;
		}
		tempPos = Camera.main.ScreenToWorldPoint(mousePos);
		transform.position = tempPos - (Vector3.forward * tempPos.z);
	}

    private void Update()
    {
        if(isFocused)
        {
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 2 * data.focusedInput.radius, 0.25f);
			
		}else
        {
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 2 * data.standardInput.radius, 0.3f);
		}
    }
}
