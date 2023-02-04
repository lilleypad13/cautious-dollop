using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSCENELOADER : MonoBehaviour
{
    public SceneBundle sceneBundle;

    public void LoadScenes()
    {
        LevelManager.LoadSceneBundle(sceneBundle);
    }
}
