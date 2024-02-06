using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
