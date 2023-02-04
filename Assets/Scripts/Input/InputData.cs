using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputParameters
{
	public float radius;
	public float speedIncrease;
	[Range(0f, 1f)] public float speedDecrease;
	public float curveLimit;
	public float curveSpeed;
	public Color color;
}

[CreateAssetMenu(fileName = "InputData", menuName = "ScriptableObjects/InputData", order = 0)]
public class InputData : ScriptableObject
{
	public InputParameters standardInput;
	public InputParameters focusedInput;
}

