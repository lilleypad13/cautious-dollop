using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Bundle", menuName = "ScriptableObjects/SceneBundle", order = 1)]
public class SceneBundle : ScriptableObject
{
    public List<string> scenesToLoad;
    public string sceneToSetActive;


}
