using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnergyMgr : MonoBehaviour
{
	public static Action EnergyDepleted;

	public int maxEnergy;
	public int drainRate;
	public int refillRate;
	public RectTransform energyBar;
	public RectTransform backing;

	private float sizeAtMax;
	private float sizePerUnit;
	private float currentEnergy;
	private bool usingEnergy = false;

	private void Start() {
		sizeAtMax = backing.rect.width;
		sizePerUnit = sizeAtMax / maxEnergy;
		currentEnergy = maxEnergy;
		energyBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeAtMax);
	}

	private void OnEnable() {
		InputController.CurrentMousePosition += RegisterMouseClick;
	}

	private void OnDisable() {
		InputController.CurrentMousePosition -= RegisterMouseClick;
	}

	private void Update() {
		if (usingEnergy && currentEnergy > 0) {
			currentEnergy -= drainRate * Time.deltaTime;
			currentEnergy = Mathf.Max(0, currentEnergy);
			if(currentEnergy == 0) {
				OutOfEnergy();
			}
		} else if (currentEnergy < maxEnergy) {
			currentEnergy += refillRate * Time.deltaTime;
			currentEnergy = Mathf.Min(maxEnergy, currentEnergy);
		}

		UpdateEnergyBar();
	}

	private void UpdateEnergyBar() {
		//If currentEnergy is 0, it throws an Invalid AABB inAABB error
		energyBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(currentEnergy, 1) * sizePerUnit);
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
