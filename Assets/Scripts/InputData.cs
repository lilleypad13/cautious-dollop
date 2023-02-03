using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputData", menuName = "ScriptableObjects/InputData", order = 0)]
public class InputData : ScriptableObject
{
	[Header("Standard Input")]
	public float standardRadius;
	public float standardPowerPercent;

	[Header("Focused Input")]
	public float focusedRadius;
	public float focusedPowerPercent;
	[Range(0f, 1f)] public float focusedSlowPercent;
}

