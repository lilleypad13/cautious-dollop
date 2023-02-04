using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HudControls : MonoBehaviour
{
	public static Action<bool> EnableHud;

	public Animator hudAnim;
	public Animator endLevelAnim;
	public RectTransform barHolder;
	public RectTransform barFill;

	private void OnEnable() {
		EnergyMgr.SetEnergyBarSize += ResizeBar;
		RootCtrl.GameWin += ShowWinUI;
		RootCtrl.GameLose += ShowLoseUI;
	}
	private void OnDisable() {
		EnergyMgr.SetEnergyBarSize -= ResizeBar;
		RootCtrl.GameWin -= ShowWinUI;
		RootCtrl.GameLose -= ShowLoseUI;
	}

	private void ResizeBar(float maxValue, float currentValue) {
		//If currentValue is 0, it throws an Invalid AABB inAABB error
		barFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(currentValue, 1) * (barHolder.rect.width / maxValue));
	}

	private void ShowHud(bool enable) {
		hudAnim.SetBool("HudOn", enable);
	}

	private void ShowWinUI() {
		endLevelAnim.SetTrigger("Win");
	}

	private void ShowLoseUI() {
		endLevelAnim.SetTrigger("Lose");
	}
}
