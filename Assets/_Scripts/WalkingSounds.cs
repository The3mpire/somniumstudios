using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WalkingSounds : MonoBehaviour {

    [SerializeField]
    private List<AudioClip> forestSounds;

    [SerializeField]
    private List<AudioClip> citySounds;


    /// <summary>
    /// Plays the a random sound based on what the floor is
    /// </summary>
    void PlayWalkingSounds() {
        switch (SceneSettings.GetFloor()) {
            case levelFloor.GRASS:
                SoundManager.instance.PlaySingle(forestSounds[Random.Range(0, forestSounds.Count)]);
                break;
            case levelFloor.CONCRETE:
                break;
        }
    }
}
