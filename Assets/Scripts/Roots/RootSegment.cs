using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSegment : MonoBehaviour
{
	public float Length {
		get { return rend.size.y - overlapFactor; }
		set {
			if (isGrowing) {
				tip.localPosition = new Vector3(tip.localPosition.x, -value + overlapFactor, tip.localPosition.z);
				rend.size = new Vector2(rend.size.x, value + overlapFactor);
				//rend.transform.localPosition = new Vector3(rend.transform.localPosition.x, -value, rend.transform.localPosition.z);
				if(coll != null) {
					coll.size = new Vector2(1, value);
					coll.offset = new Vector2(coll.offset.x, -value / 2f);
				}
			}
		}
	}

	public bool IsGrowing { get { return isGrowing; } }
	public int Depth { get; set; }
	public float Angle { get { return currentAngle; } }

	public float overlapFactor = 0.05f;
	public SpriteRenderer rend;
	public BoxCollider2D coll;
	public Color disabledColor;
	public float colorChangeSpeed;
	public Transform tip;
	private bool isGrowing = true;
	private float initialRot;
	private float currentAngle;

	private void Awake() {
		Length = overlapFactor;
	}

	public void SetInitialRotation(float angle) {
		initialRot = angle;
		currentAngle = angle;
	}

	public void BendTowards(Vector3 targetPos, float angleLimit, float speed) {
		Vector3 dir = targetPos - transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		float lowerLimit = -angleLimit + initialRot;
		float upperLimit = angleLimit + initialRot;

		angle += 90;
		if(angle > 180 + initialRot) { angle = lowerLimit; }
		angle = Mathf.Clamp(angle, lowerLimit, upperLimit);
		angle = Mathf.Lerp(currentAngle, angle, Time.deltaTime * speed);

		currentAngle = angle;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public void EnableGrowth(bool enable) {
		isGrowing = enable;
		if(coll != null) { Destroy(coll); }
	}

	public void ChangeColor() {
		StartCoroutine(DoChangeColor());
	}

	private IEnumerator DoChangeColor() {
		Color initial = rend.color;
		for(float t = 0; t <= 1; t += Time.deltaTime * colorChangeSpeed) {
			rend.color = Color.Lerp(initial, disabledColor, t);
			yield return null;
		}
	}
}
