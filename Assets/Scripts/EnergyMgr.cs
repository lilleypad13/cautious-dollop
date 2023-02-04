using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyMgr : MonoBehaviour
{
	public int maxEnergy;
	public int drainRate;
	public int refillRate;
	public Slider energyBar;

	private float currentEnergy;
	private bool usingEnergy = false;

	private void Start() {
		currentEnergy = maxEnergy;
		energyBar.maxValue = maxEnergy;
		energyBar.value = maxEnergy;
	}

	private void OnEnable() {
		InputController.CurrentMousePosition += RegisterMouseClick;
	}

	private void OnDisable() {
		InputController.CurrentMousePosition -= RegisterMouseClick;
	}

	private void Update() {
		if (usingEnergy) {
			currentEnergy -= drainRate * Time.deltaTime;
			currentEnergy = Mathf.Max(0, currentEnergy);
		} else if (currentEnergy < maxEnergy) {
			currentEnergy += refillRate * Time.deltaTime;
			currentEnergy = Mathf.Min(maxEnergy, currentEnergy);
		}

		UpdateEnergyBar();
	}

	private void UpdateEnergyBar() {
		energyBar.value = currentEnergy;
	}

	private void RegisterMouseClick(Vector2 mousePos, MouseInput inputType) {
		if(inputType == MouseInput.LeftClickDown) {
			usingEnergy = true;
		} else if (inputType == MouseInput.None && usingEnergy) {
			usingEnergy = false;
		}
	}

	
}
