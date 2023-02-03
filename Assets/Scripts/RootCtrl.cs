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
	[Range(0f, 90f)] public float branchAngleVariance;
	public RootSegment segmentPrefab;

	private List<RootSegment> rootList = new List<RootSegment>();
	private List<RootSegment> tempList = new List<RootSegment>();
	private int segmentsSinceBranch = 0;

	private void Start() {
		rootList.Add(CreateSegment(transform.position));
	}

	private void Update() {
		foreach (var root in rootList) {
			root.Length += growthSpeed * Time.deltaTime;
			if(root.Length >= segmentLength) {
				root.EnableGrowth(false);
				if(root.transform.parent.childCount >= segmentsBetweenBranches.x) {
					float chance = 1 / (segmentsBetweenBranches.y - segmentsBetweenBranches.x + 1.0f);
					Debug.Log($"{root.transform.parent.childCount} since branch, current chance {chance * (root.transform.parent.childCount - segmentsBetweenBranches.x + 1.0f)}");
					if (Random.Range(0f, 1f) <= chance * (root.transform.parent.childCount - segmentsBetweenBranches.x + 1.0f)) {
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
		return Instantiate(segmentPrefab, startPos, Quaternion.AngleAxis(Random.Range(-angleVariance, angleVariance), Vector3.forward), BranchParent());
	}

	private RootSegment CreateSegment(RootSegment parent) {
		Vector3 angle = Quaternion.AngleAxis(Random.Range(-angleVariance, angleVariance), parent.transform.forward).eulerAngles + parent.transform.eulerAngles;
		return Instantiate(segmentPrefab, parent.tip.position, Quaternion.Euler(angle), parent.transform.parent);
	}

	private void CreateBranches(RootSegment parent, List<RootSegment> addList) {
		float angle = Random.Range(0f, branchAngleVariance);
		Debug.Log($"Creating branches at angle {angle}");
		addList.Add(Instantiate(segmentPrefab, parent.tip.position, 
			Quaternion.Euler(Quaternion.AngleAxis(angle, parent.transform.forward).eulerAngles + parent.transform.eulerAngles), 
			BranchParent()));
		addList.Add(Instantiate(segmentPrefab, parent.tip.position, 
			Quaternion.Euler(Quaternion.AngleAxis(-angle, parent.transform.forward).eulerAngles + parent.transform.eulerAngles), 
			BranchParent()));
	}

	private Transform BranchParent() {
		GameObject g = new GameObject("Branch");
		g.transform.parent = transform;
		g.transform.localPosition = Vector3.zero;
		return g.transform;
	}
}
