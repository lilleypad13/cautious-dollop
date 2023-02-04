using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RootCtrl : MonoBehaviour
{
	public static Action GameWin;
	public static Action GameLose;

	public InputData data;
	public float segmentLength;
	public float growthSpeed;
	public Vector2Int segmentsBetweenBranches;
	[Range(0f, 90f)] public float angleVariance;
	[Range(0f, 90f)] public float branchAngleVarianceMin;
	[Range(0f, 90f)] public float branchAngleVarianceMax;
	public RootSegment segmentPrefab;
	public float branchChanceDecay;

	private InputParameters currentInputParams;

	private float mod = 1;
	private List<RootSegment> rootList = new List<RootSegment>();
	private List<RootSegment> tempList = new List<RootSegment>();
	private Vector2Int localSegmentsBetween;

	private Vector2 mousePos;

	int nextBranch = 0;

	private bool gameOver = false;

	private void Start() {
		currentInputParams = data.standardInput;
		rootList.Add(CreateFirstSegment(transform.position));
	}

	private void OnEnable() {
		InputController.CurrentMousePosition += ToggleFocus;
		PickupMgr.AllPickupsCollected += WinGame;
	}

	private void OnDisable() {
		InputController.CurrentMousePosition -= ToggleFocus;
		PickupMgr.AllPickupsCollected -= WinGame;
	}

	private void Update() {
		if (!gameOver) {
			foreach (var root in rootList) {
				if (RootInRange(root)) {
					mod = 1 + currentInputParams.speedIncrease;
					root.BendTowards(mousePos, currentInputParams.curveLimit, currentInputParams.curveSpeed);
				} else {
					mod = Mathf.Max(0f, 1 - currentInputParams.speedDecrease);
				}
				root.Length += growthSpeed * Time.deltaTime * mod;
				if (root.Length >= segmentLength) {
					root.EnableGrowth(false);
					localSegmentsBetween = BranchRange(root.Depth);
					if (root.transform.parent.childCount >= localSegmentsBetween.x) {
						if (UnityEngine.Random.Range(0f, 1f) <= BranchChance(localSegmentsBetween, root.transform.parent.childCount)) {
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
			if(rootList.Count == 0) {
				LoseGame();
			}
		}
	}

	private void LoseGame() {
		Debug.Log("You Lose!");
		GameLose?.Invoke();
		gameOver = true;
	}

	private void WinGame() {
		Debug.Log("You Win!");
		GameWin?.Invoke();
		gameOver = true;
	}

	private RootSegment CreateFirstSegment(Vector3 startPos) {
		float angle = UnityEngine.Random.Range(-angleVariance, angleVariance);
		RootSegment newSegment = Instantiate(segmentPrefab,
			startPos,
			Quaternion.AngleAxis(angle, Vector3.forward),
			BranchParent(nextBranch));
		Collider2D col = newSegment.GetComponentInChildren<Collider2D>();
		if (col != null) { Destroy(col); }
		nextBranch++;
		newSegment.SetInitialRotation(angle);
		newSegment.Depth = 0;
		return newSegment;
	}

	private RootSegment CreateSegment(RootSegment parent) {
		float angle = UnityEngine.Random.Range(-angleVariance, angleVariance);
		RootSegment newSegment = Instantiate(segmentPrefab,
			parent.tip.position,
			Quaternion.Euler(Quaternion.AngleAxis(angle, parent.transform.forward).eulerAngles + parent.transform.eulerAngles),
			parent.transform.parent);
		newSegment.Depth = parent.Depth;
		newSegment.SetInitialRotation(parent.Angle + angle);
		return newSegment;
	}

	private void CreateBranches(RootSegment parent, List<RootSegment> addList) {
		float angle = UnityEngine.Random.Range(branchAngleVarianceMin, branchAngleVarianceMax);
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
			currentInputParams = data.focusedInput;
		} else if (inputType == MouseInput.None && currentInputParams == data.focusedInput) {
			currentInputParams = data.standardInput;
		}
		this.mousePos = Camera.main.ScreenToWorldPoint(mousePos);
	}

	private bool RootInRange(RootSegment root) {
		//Need to detect if any part is within the radius, not just the ends
		Vector2 pointOnLine = ClosestPointOnLine(root.transform.position, root.tip.position, mousePos);
		return Vector2.Distance(mousePos, pointOnLine) <= currentInputParams.radius;
	}

	private Vector2 ClosestPointOnLine(Vector2 endpointOne, Vector2 endpointTwo, Vector2 pointToCheck) {
		Vector2 v = endpointTwo - endpointOne;
		Vector2 u = endpointOne - pointToCheck;

		float t = -(Vector2.Dot(v, u) / Vector2.Dot(v, v));
		return Vector2.Lerp(endpointOne, endpointTwo, t);
	}
}
