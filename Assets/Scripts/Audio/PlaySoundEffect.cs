using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    public string SFXName;

    public void PlaySound()
    {
        AudioManager.Instance.PlaySFX(SFXName);
    }
}
