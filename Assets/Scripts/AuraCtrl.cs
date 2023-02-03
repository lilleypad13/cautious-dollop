using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraCtrl : MonoBehaviour
{
	public InputData data;

	private Vector3 tempPos;
	private bool isFocused = false;

	private void OnEnable() {
		//Data stores radius, but scale is diameter
		transform.localScale = Vector3.one * 2 * data.standardRadius;
		InputController.CurrentMousePosition += GetMousePos;
	}

	private void OnDisable() {
		InputController.CurrentMousePosition -= GetMousePos;
	}

	private void GetMousePos(Vector2 mousePos, MouseInput inputType) {
		if(inputType == MouseInput.LeftClickDown) {
			isFocused = true;
			transform.localScale = Vector3.one * 2 * data.focusedRadius;
		} else if (inputType == MouseInput.None && isFocused) {
			isFocused = false;
			transform.localScale = Vector3.one * 2 * data.standardRadius;
		}
		tempPos = Camera.main.ScreenToWorldPoint(mousePos);
		transform.position = tempPos - (Vector3.forward * tempPos.z);
	}
}
