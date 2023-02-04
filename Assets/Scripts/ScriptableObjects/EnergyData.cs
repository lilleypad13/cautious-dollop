using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnergyData", menuName = "ScriptableObjects/EnergyData", order = 1)]
public class EnergyData : ScriptableObject
{
	public float maxEnergy;
	public float drainRate;
	public float refillRate;
}
