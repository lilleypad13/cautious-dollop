using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnergyMgr : MonoBehaviour
{
	public static Action EnergyDepleted;
	public static Action<float, float> SetEnergyBarSize;

	public EnergyData data;
	
	private float currentEnergy;
	private bool usingEnergy = false;

	private void Start() {
		currentEnergy = data.maxEnergy;
	}

	private void OnEnable() {
		InputController.CurrentMousePosition += RegisterMouseClick;
		SetEnergyBarSize?.Invoke(data.maxEnergy, currentEnergy);
	}

	private void OnDisable() {
		InputController.CurrentMousePosition -= RegisterMouseClick;
	}

	private void Update() {
		if (usingEnergy && currentEnergy > 0) {
			currentEnergy -= data.drainRate * Time.deltaTime;
			currentEnergy = Mathf.Max(0, currentEnergy);
			if(currentEnergy == 0) {
				OutOfEnergy();
			}
		} else if (currentEnergy < data.maxEnergy) {
			currentEnergy += data.refillRate * Time.deltaTime;
			currentEnergy = Mathf.Min(data.maxEnergy, currentEnergy);
		}

		SetEnergyBarSize?.Invoke(data.maxEnergy, currentEnergy);
	}

	private void OutOfEnergy() {
		usingEnergy = false;
		EnergyDepleted?.Invoke();
	}

	private void RegisterMouseClick(Vector2 mousePos, MouseInput inputType) {
		if(inputType == MouseInput.LeftClickDown) {
			usingEnergy = true;
		} else if (inputType == MouseInput.None && usingEnergy) {
			usingEnergy = false;
		}
	}

	
}
