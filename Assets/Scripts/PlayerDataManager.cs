using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;


    public int currentLevelProgress = 0;
    public int CurrentLevelProgress { get => currentLevelProgress; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void UpdateLevelProgress(int highestLevel)
    {
        currentLevelProgress = highestLevel;
    }
}
