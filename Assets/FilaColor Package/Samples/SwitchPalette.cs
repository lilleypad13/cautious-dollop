using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPalette : MonoBehaviour
{
	int index = 0;

	public void SwitchToDefault() {
		PaletteManager.SetChosenPalette("Default");
	}

	public void SwitchToAutumn() {
		PaletteManager.SetChosenPalette("Autumn");
	}

	public void SwitchToSpring() {
		PaletteManager.SetChosenPalette("Spring");
	}

	public void SwitchToWinter() {
		PaletteManager.SetChosenPalette("Winter");
	}

	public void CyclePalette() {
		index = (index + 1) % PaletteManager.GetPalettes().Length;
		PaletteManager.SetChosenPalette(index);
	}
}
