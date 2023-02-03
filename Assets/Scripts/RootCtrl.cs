using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RootCtrl : MonoBehaviour
{
	public float segmentLength;
	public float growthSpeed;
	public float branchChance;
	[Range(0f, 90f)] public float angleVariance;
	[Range(0f, 90f)] public float branchAngleVariance;
	public RootSegment segmentPrefab;

	private List<RootSegment> rootList = new List<RootSegment>();
	private List<RootSegment> tempList = new List<RootSegment>();

	private void Start() {
		rootList.Add(CreateSegment(transform.position));
	}

	private void Update() {
		foreach (var root in rootList) {
			root.Length += growthSpeed * Time.deltaTime;
			if(root.Length >= segmentLength) {
				root.EnableGrowth(false);
				tempList.Add(CreateSegment(root.tip.position));
			}
		}
		rootList = rootList.Concat(tempList).Where(x => x.IsGrowing).ToList();
		tempList.Clear();
	}

	private RootSegment CreateSegment(Vector3 startPos) {
		Quaternion rot = Quaternion.AngleAxis(Random.Range(-angleVariance, angleVariance), Vector3.forward);
		RootSegment newSegment = Instantiate(segmentPrefab, startPos, rot, transform);
		return newSegment;
	}

	private RootSegment CreateSegment(RootSegment parent) {
		Quaternion rot = Quaternion.AngleAxis(Random.Range(-angleVariance, angleVariance), parent.transform.right);
		RootSegment newSegment = Instantiate(segmentPrefab, parent.tip.position, rot, transform);
		return newSegment;
	}
}
