using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RootCtrl : MonoBehaviour
{
	public float segmentLength;
	public float growthSpeed;
	public Vector2Int segmentsBetweenBranches;
	[Range(0f, 90f)] public float angleVariance;
	[Range(0f, 90f)] public float branchAngleVarianceMin;
	[Range(0f, 90f)] public float branchAngleVarianceMax;
	public RootSegment segmentPrefab;
	public float branchChanceDecay;

	private List<RootSegment> rootList = new List<RootSegment>();
	private List<RootSegment> tempList = new List<RootSegment>();
	private Vector2Int localSegmentsBetween;

	private void Start() {
		rootList.Add(CreateSegment(transform.position));
	}

	private void Update() {
		foreach (var root in rootList) {
			root.Length += growthSpeed * Time.deltaTime;
			if(root.Length >= segmentLength) {
				root.EnableGrowth(false);
				localSegmentsBetween = BranchRange(root.Depth);
				if(root.transform.parent.childCount >= localSegmentsBetween.x) {
					if (Random.Range(0f, 1f) <= BranchChance(localSegmentsBetween, root.transform.parent.childCount)) {
						Debug.Log($"Creating branches at {root.transform.parent.childCount} (between {localSegmentsBetween.x} and {localSegmentsBetween.y}, depth {root.Depth})");
						CreateBranches(root, tempList);
					} else {
						tempList.Add(CreateSegment(root));
					}
				} else {
					tempList.Add(CreateSegment(root));
				}
			}
		}
		rootList = rootList.Concat(tempList).Where(x => x.IsGrowing).ToList();
		tempList.Clear();
	}

	private RootSegment CreateSegment(Vector3 startPos) {
		RootSegment newSegment = Instantiate(segmentPrefab, startPos, Quaternion.AngleAxis(Random.Range(-angleVariance, angleVariance), Vector3.forward), BranchParent());
		newSegment.Depth = 0;
		return newSegment;
	}

	private RootSegment CreateSegment(RootSegment parent) {
		Vector3 angle = Quaternion.AngleAxis(Random.Range(-angleVariance, angleVariance), parent.transform.forward).eulerAngles + parent.transform.eulerAngles;
		RootSegment newSegment =  Instantiate(segmentPrefab, parent.tip.position, Quaternion.Euler(angle), parent.transform.parent);
		newSegment.Depth = parent.Depth;
		return newSegment;
	}

	private void CreateBranches(RootSegment parent, List<RootSegment> addList) {
		float angle = Random.Range(branchAngleVarianceMin, branchAngleVarianceMax);
		
		RootSegment branch1 = Instantiate(segmentPrefab, parent.tip.position,
			Quaternion.Euler(Quaternion.AngleAxis(angle, parent.transform.forward).eulerAngles + parent.transform.eulerAngles),
			BranchParent());
		branch1.Depth = parent.Depth + 1;
		addList.Add(branch1);

		RootSegment branch2 = Instantiate(segmentPrefab, parent.tip.position, 
			Quaternion.Euler(Quaternion.AngleAxis(-angle, parent.transform.forward).eulerAngles + parent.transform.eulerAngles), 
			BranchParent());
		branch2.Depth = parent.Depth + 1;
		addList.Add(branch2);
	}

	private Transform BranchParent() {
		GameObject g = new GameObject("Branch");
		g.transform.parent = transform;
		g.transform.localPosition = Vector3.zero;
		return g.transform;
	}

	private float BranchChance(Vector2Int range, int count) {
		return (1 / (range.y - range.x + 1.0f)) * (count - range.x + 1);
	}

	private Vector2Int BranchRange(int depth) {
		int mod = Mathf.FloorToInt(depth * branchChanceDecay);
		return segmentsBetweenBranches + new Vector2Int(mod, mod);
	}
}
