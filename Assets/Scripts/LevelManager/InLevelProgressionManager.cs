using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLevelProgressionManager : MonoBehaviour
{
    [Tooltip("Int related to player progression. Each level should have its own int that update player progression to this value when WINNING THIS LEVEL.")]
    public int completingThisLevelProgression = 1;

    private void OnEnable()
    {
        RootCtrl.GameWin += WinLevelProgressionUpdate;
    }

    private void OnDisable()
    {
        RootCtrl.GameWin -= WinLevelProgressionUpdate;
    }

    public void WinLevelProgressionUpdate()
    {
        PlayerDataManager.Instance.UpdateLevelProgress(completingThisLevelProgression);
    }
}
