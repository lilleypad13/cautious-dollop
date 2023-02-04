using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StaticObstacle : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Root")) {
			RootSegment segment = collision.gameObject.GetComponentInParent<RootSegment>();
			if(segment != null) {
				segment.EnableGrowth(false);
				segment.ChangeColor();
			}
		}
	}
}
