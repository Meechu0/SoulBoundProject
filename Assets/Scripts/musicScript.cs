using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicScript : MonoBehaviour
{
 
     public AudioSource MenuMusic;
    public AudioSource gameMusic;

    private void Awake()
     {
        DontDestroyOnLoad(transform.gameObject);
        PlayMenuMusic();
     }
 
     public void PlayMenuMusic()
     {
        MenuMusic.Play();
        gameMusic.Stop();
     }
 
     public void StopMenuMusic()
     {
        MenuMusic.Stop();
        gameMusic.Play();
    }
 
}
