using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PickupMgr : MonoBehaviour
{
	public static Action AllPickupsCollected;

	private int pickupNum;

	private void OnEnable() {
		pickupNum = FindObjectsOfType<Pickup>().Length;
		Pickup.PickupCollected += PickupCollected;
	}

	private void OnDisable() {
		Pickup.PickupCollected -= PickupCollected;
	}

	private void PickupCollected() {
		pickupNum--;
		if(pickupNum == 0) {
			AllPickupsCollected?.Invoke();
		}
	}
}
