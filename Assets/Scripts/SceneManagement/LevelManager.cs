using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //public SceneBundle initializationBundle;
    public SceneBundle currentlyLoadedBundle;
    public static LevelManager instance;

    public string PersistentSceneName = "Boot";


    List<AsyncOperation> unloadOperations = new List<AsyncOperation>();
    List<AsyncOperation> loadOperations = new List<AsyncOperation>();

    private void Awake()
    {
        instance = this;
    }



    public static void LoadSceneBundle(SceneBundle sceneBundle)
    {
        instance.StartCoroutine(instance.LoadSequence(sceneBundle));
    }


    private IEnumerator LoadSequence(SceneBundle sceneBundle)
    {
        // Only find scenes to unload if there is an existing level bundle
        if(currentlyLoadedBundle != null)
        {
            // Get list of active scenes
            string[] currentlyLoadedScenes = GetLoadedScenes();

            // Determine overlap with scenes to load
            // TBD

            Debug.Log($"Unloading {currentlyLoadedScenes.Length}");
            // Unload excess scenes
            yield return UnloadCurrentScenes(currentlyLoadedScenes);
            yield return null;
        }

        // Load new scenes
        Debug.Log($"Loading{sceneBundle.scenesToLoad.Count}");
        yield return LoadNextScenes(sceneBundle);
        yield return null;

        currentlyLoadedBundle = sceneBundle;
        yield return null;
    }

    private string[] GetLoadedScenes()
    {
        int loadedScenesCount = currentlyLoadedBundle.scenesToLoad.Count;
        string[] loadedScenes = new string[loadedScenesCount];


        for (int i = 0; i < loadedScenesCount; i++)
        {
            loadedScenes[i] = currentlyLoadedBundle.scenesToLoad[i];
        }

        return loadedScenes;
    }




    private IEnumerator UnloadCurrentScenes(string[] currentlyLoadedScenes)
    {
        unloadOperations = new List<AsyncOperation>();

        foreach (string sceneName in currentlyLoadedScenes)
        {
            if(sceneName != PersistentSceneName)
            {
                unloadOperations.Add(SceneManager.UnloadSceneAsync(sceneName));
            }
        }

        if(unloadOperations.Count > 0)
        {
            bool unloadFinished;
            do
            {
                unloadFinished = true;
                foreach (AsyncOperation op in unloadOperations)
                {
                    unloadFinished = unloadFinished && op.isDone;
                }
                yield return null;

            } while (!unloadFinished);
        }
    }


    private IEnumerator LoadNextScenes(SceneBundle sceneBundle)
    {
        loadOperations = new List<AsyncOperation>();

        foreach (string sceneName in sceneBundle.scenesToLoad)
        {
            loadOperations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
        }

        bool loadFinished;
        do
        {
            loadFinished = true;
            foreach (AsyncOperation op in loadOperations)
            {
                loadFinished = loadFinished && op.isDone;
            }
            yield return null;

        } while (!loadFinished);
    }

}
