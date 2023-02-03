using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RootCtrl : MonoBehaviour
{
	public InputData data;
	public float segmentLength;
	public float growthSpeed;
	public Vector2Int segmentsBetweenBranches;
	[Range(0f, 90f)] public float angleVariance;
	[Range(0f, 90f)] public float branchAngleVarianceMin;
	[Range(0f, 90f)] public float branchAngleVarianceMax;
	public RootSegment segmentPrefab;
	public float branchChanceDecay;

	private bool isFocusing = false;

	private float mod = 1;
	private List<RootSegment> rootList = new List<RootSegment>();
	private List<RootSegment> tempList = new List<RootSegment>();
	private Vector2Int localSegmentsBetween;

	private Vector2 mousePos;

	int nextBranch = 0;

	private void Start() {
		rootList.Add(CreateFirstSegment(transform.position));
	}

	private void OnEnable() {
		InputController.CurrentMousePosition += ToggleFocus;
	}

	private void OnDisable() {
		InputController.CurrentMousePosition -= ToggleFocus;
	}

	private void Update() {
		foreach (var root in rootList) {
			if (RootInRange(root)) {
				mod = 1 + (isFocusing ? data.focusedPowerPercent : data.standardPowerPercent);
				root.rend.color = Color.red;
				root.BendTowards(mousePos, 22.5f, 0f);
			} else if (isFocusing) {
				mod = Mathf.Max(0f, 1 - data.focusedSlowPercent);
				root.rend.color = Color.blue;
			} else {
				mod = 1;
				root.rend.color = Color.white;
			}

			root.Length += growthSpeed * Time.deltaTime * mod;
			if (root.Length >= segmentLength) {
				root.EnableGrowth(false);
				root.rend.color = Color.white;
				localSegmentsBetween = BranchRange(root.Depth);
				if (root.transform.parent.childCount >= localSegmentsBetween.x) {
					if (Random.Range(0f, 1f) <= BranchChance(localSegmentsBetween, root.transform.parent.childCount)) {
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

	private RootSegment CreateFirstSegment(Vector3 startPos) {
		float angle = Random.Range(-angleVariance, angleVariance);
		RootSegment newSegment = Instantiate(segmentPrefab,
			startPos,
			Quaternion.AngleAxis(angle, Vector3.forward),
			BranchParent(nextBranch));
		nextBranch++;
		newSegment.SetInitialRotation(angle);
		newSegment.Depth = 0;
		return newSegment;
	}

	private RootSegment CreateSegment(RootSegment parent) {
		float angle = Random.Range(-angleVariance, angleVariance);
		RootSegment newSegment = Instantiate(segmentPrefab,
			parent.tip.position,
			Quaternion.Euler(Quaternion.AngleAxis(angle, parent.transform.forward).eulerAngles + parent.transform.eulerAngles),
			parent.transform.parent);
		newSegment.Depth = parent.Depth;
		newSegment.SetInitialRotation(parent.Angle + angle);
		return newSegment;
	}

	private void CreateBranches(RootSegment parent, List<RootSegment> addList) {
		float angle = Random.Range(branchAngleVarianceMin, branchAngleVarianceMax);
		RootSegment branch1 = Instantiate(segmentPrefab,
			parent.tip.position,
			Quaternion.Euler(Quaternion.AngleAxis(angle, parent.transform.forward).eulerAngles + parent.transform.eulerAngles),
			BranchParent(nextBranch));
		branch1.Depth = parent.Depth + 1;
		nextBranch++;
		branch1.SetInitialRotation(parent.Angle + angle);
		addList.Add(branch1);

		RootSegment branch2 = Instantiate(segmentPrefab,
			parent.tip.position,
			Quaternion.Euler(Quaternion.AngleAxis(-angle, parent.transform.forward).eulerAngles + parent.transform.eulerAngles),
			BranchParent(nextBranch));
		branch2.Depth = parent.Depth + 1;
		nextBranch++;
		branch2.SetInitialRotation(parent.Angle - angle);
		addList.Add(branch2);
	}

	private Transform BranchParent(int id) {
		GameObject g = new GameObject($"Branch_{id}");
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

	private int GetBranchIndex(Transform branch) {
		try {
			return int.Parse(branch.name.Split('_')[1]);
		} catch {
			return -1;
		}
	}

	private void ToggleFocus(Vector2 mousePos, MouseInput inputType) {
		if (inputType == MouseInput.LeftClickDown) {
			isFocusing = true;
		} else if (inputType == MouseInput.None) {
			if (isFocusing) {
				isFocusing = false;
			}
		}
		this.mousePos = Camera.main.ScreenToWorldPoint(mousePos);
	}

	private bool RootInRange(RootSegment root) {
		float radius = isFocusing ? data.focusedRadius : data.standardRadius;
		//Need to detect if any part is within the radius, not just the ends
		Vector2 pointOnLine = ClosestPointOnLine(root.transform.position, root.tip.position, mousePos);
		return Vector2.Distance(mousePos, pointOnLine) <= radius;
	}

	private Vector2 ClosestPointOnLine(Vector2 endpointOne, Vector2 endpointTwo, Vector2 pointToCheck) {
		Vector2 v = endpointTwo - endpointOne;
		Vector2 u = endpointOne - pointToCheck;

		float t = -(Vector2.Dot(v, u) / Vector2.Dot(v, v));
		return Vector2.Lerp(endpointOne, endpointTwo, t);
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(RootCtrl))]
	public class RootCtrlEditor : Editor
	{
		private RootCtrl script;

		private void OnEnable() {
			script = (RootCtrl)target;
		}

		private void OnSceneGUI() {
			if(script.mousePos != null) {
				float radius = script.isFocusing ? script.data.focusedRadius : script.data.standardRadius;
				Handles.color = Color.black;
				Handles.DrawSolidArc(script.mousePos, Vector3.forward, Vector3.up, 360, .1f);
				Handles.color = Color.yellow;
				Handles.DrawWireArc(script.mousePos, Vector3.forward, Vector3.up, 360, radius);
			}
		}
	}
#endif
}
