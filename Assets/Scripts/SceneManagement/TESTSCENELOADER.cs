using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSCENELOADER : MonoBehaviour
{
    public SceneBundle sceneBundle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadScenes();
        }
    }

    public void LoadScenes()
    {
        LevelManager.LoadSceneBundle(sceneBundle);
    }
}
