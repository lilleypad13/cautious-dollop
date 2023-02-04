using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    public Button[] levelSelectButtons;

    private void Start()
    {
        if(PlayerDataManager.Instance.CurrentLevelProgress < levelSelectButtons.Length)
        {
            for (int i = levelSelectButtons.Length - 1; i > PlayerDataManager.Instance.CurrentLevelProgress; i--)
            {
                levelSelectButtons[i].interactable = false;
            }
        }
    }
}
