using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Animator sceneAnimator;

    private void Start()
    {
        PlayCameraTransition();
    }

    private void PlayCameraTransition()
    {
        sceneAnimator.SetBool("Play", true);
    }
}
