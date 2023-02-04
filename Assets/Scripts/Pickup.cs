using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pickup : MonoBehaviour
{
	public static Action PickupCollected;

	public Animator anim;

	private bool hasBeenCollected = false;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (!hasBeenCollected && collision.CompareTag("Root")) {
			hasBeenCollected = true;
			PickupCollected?.Invoke();
			Collider2D col = GetComponent<Collider2D>();
			if(col != null) { Destroy(col); }
			anim.SetTrigger("ToOff");
		}
	}
}
