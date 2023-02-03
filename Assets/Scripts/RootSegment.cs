using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSegment : MonoBehaviour
{
	public float Length {
		get { return transform.localScale.y; }
		set { transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z); }
	}

	public bool IsGrowing { get { return isGrowing; } }
	public int Depth { get; set; }

	public SpriteRenderer rend;

	public Transform tip;
	private bool isGrowing = true;

	public void EnableGrowth(bool enable) {
		isGrowing = enable;
	}
}
