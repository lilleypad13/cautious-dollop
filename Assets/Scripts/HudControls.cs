using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HudControls : MonoBehaviour
{
	public static Action<bool> EnableHud;

	public bool isLastLevel = false;
	public Animator hudAnim;
	public Animator endLevelAnim;
	public RectTransform barHolder;
	public RectTransform barFill;

	public float waitTimeForHUD = 1.0f;

	private void OnEnable() {
		EnergyMgr.SetEnergyBarSize += ResizeBar;
		RootCtrl.GameWin += ShowWinUI;
		RootCtrl.GameLose += ShowLoseUI;

		StartCoroutine(WaitToShowHUD());
	}
	private void OnDisable() {
		EnergyMgr.SetEnergyBarSize -= ResizeBar;
		RootCtrl.GameWin -= ShowWinUI;
		RootCtrl.GameLose -= ShowLoseUI;

		StopCoroutine(WaitToShowHUD());
	}

	private void ResizeBar(float maxValue, float currentValue) {
		//If currentValue is 0, it throws an Invalid AABB inAABB error
		barFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(currentValue, 1) * (barHolder.rect.width / maxValue));
	}

	private void ShowHud(bool enable) {
		Debug.Log($"Calling ShowHud({enable})");
		hudAnim.SetBool("HudOn", enable);
	}

	private void ShowWinUI() {
		ShowHud(false);
		endLevelAnim.SetBool("IsLastLevel", isLastLevel);
		endLevelAnim.SetTrigger("Win");
	}

	private void ShowLoseUI() {
		ShowHud(false);
		endLevelAnim.SetTrigger("Lose");
	}

	private IEnumerator WaitToShowHUD()
    {
		yield return new WaitForSeconds(waitTimeForHUD);
		ShowHud(true);
    }
}
