using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource placeSound;
    AudioSource bonusSound;
    AudioSource captureSound;

    // Start is called before the first frame update
    void Start()
    {
        placeSound = transform.GetChild(0).GetComponent<AudioSource>();
        bonusSound = transform.GetChild(1).GetComponent<AudioSource>();
        captureSound = transform.GetChild(2).GetComponent<AudioSource>();
    }

    public void PlayDiePlacement(bool bonus)
    {
        if (bonus)
            bonusSound.Play();
        else
            placeSound.Play();
    }

    public void PlayCaptureSound()
    {
        captureSound.Play();
    }
}
