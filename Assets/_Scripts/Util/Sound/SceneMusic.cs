using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusic : MonoBehaviour {

    [SerializeField]
    private AudioClip music;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    /// <summary>
    /// Tell the sound manager to play our music
    /// </summary>
    void OnLevelWasLoaded() {
        SoundManager.instance.SceneMusic(music);
    }
}