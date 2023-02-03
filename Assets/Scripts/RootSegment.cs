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
	public float Angle { get { return currentAngle; } }

	public SpriteRenderer rend;

	public Transform tip;
	private bool isGrowing = true;
	private float initialRot;
	private float currentAngle;

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
	}
	

	public static Vector3 DirFromAngle(float degrees, bool globalAngle, Transform relativeTo) {
		if (!globalAngle) {
			degrees += relativeTo.eulerAngles.z;
		}
		return new Vector3(Mathf.Sin(degrees * Mathf.Deg2Rad), Mathf.Cos(degrees * Mathf.Deg2Rad));
	}

	public static float AngleBetween(Vector3 center, Vector3 pointA, Vector3 pointB) {
		float distCtoA = Vector3.Distance(center, pointA);
		float distCtoB = Vector3.Distance(center, pointB);
		float distAtoB = Vector3.Distance(pointA, pointB);
		float numerator = Mathf.Pow(distCtoA, 2) + Mathf.Pow(distCtoB, 2) - Mathf.Pow(distAtoB, 2);
		float denominator = 2 * distCtoA * distCtoB;
		return Mathf.Acos(numerator / denominator) * Mathf.Rad2Deg;
	}
}
