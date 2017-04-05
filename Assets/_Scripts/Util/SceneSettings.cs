using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour {
    [HideInInspector]
    public static SceneSettings instance;

    [Tooltip("Whether this level is a puzzle or not")]
    public bool isPuzzle;

    [Tooltip("The song this scene should play")]
    public AudioClip music;

    void Awake() {
        instance = this;
    }

    /// <summary>
    /// Tell the sound manager to play our music
    /// </summary>
    void OnLevelWasLoaded() {
        SoundManager.instance.SceneMusic(music);
    }

    /// <summary>
    /// Get the correct music for this scene
    /// </summary>
    /// <returns></returns>
    public static AudioClip GetMusic() {
        return instance.music;
    }


    /// <summary>
    /// Get whether this scene is a puzzle or not
    /// </summary>
    /// <returns></returns>
    public static bool getIsPuzzle() {
        return instance.isPuzzle;
    }
}
