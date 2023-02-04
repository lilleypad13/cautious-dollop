using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pickup : MonoBehaviour
{
	public static Action PickupCollected;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Root")) {
			PickupCollected?.Invoke();
			Destroy(gameObject);
		}
	}
}
