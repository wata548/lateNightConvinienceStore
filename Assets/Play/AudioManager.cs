using System;
using System.IO;
using UnityEngine;

public class AudioManager: MonoBehaviour {

    public static AudioManager Instance { get; private set; } = null;
    private AudioSource source;
    
    private void Awake() {
        
        source = GetComponent<AudioSource>();
        if (Instance == null)
            Instance = this;
        
        else if (Instance != this)
            Destroy(this);
    }

    public void PlaySound(string audioSource) {

        var rawData = File.ReadAllBytes(audioSource);
        //AudioClip clip = ;
        //clip
    }
}