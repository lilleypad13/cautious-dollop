using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUiControls : MonoBehaviour
{
    public Button guideButon;

    public Animator sceneAnimator;

    private void Start()
    {
        guideButon.onClick.AddListener(ToggleSceneAnimator);
    }

    private void ToggleSceneAnimator()
    {
        bool tempBool = sceneAnimator.GetBool("Play");
        sceneAnimator.SetBool("Play", !tempBool);
    }
}
