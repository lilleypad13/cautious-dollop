using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlantPlacer : MonoBehaviour
{
	public static Action StartGame;
	public static Action<Vector3> PlaceObject;

	private bool levelStarted = false;

	private void OnEnable() {
		InputController.CurrentMousePosition += TriggerPlaceObject;
		levelStarted = false;
	}

	private void OnDisable() {
		InputController.CurrentMousePosition -= TriggerPlaceObject;
		levelStarted = false;
	}

	private void TriggerPlaceObject(Vector2 mousePos, MouseInput inputType) {
		if(inputType == MouseInput.LeftClickDown && !levelStarted) {
			levelStarted = true;
			InputController.CurrentMousePosition -= TriggerPlaceObject;
			Vector3 startPos = mousePos;
			PlaceObject?.Invoke(startPos);
			StartGame?.Invoke();
		}
	}
}
