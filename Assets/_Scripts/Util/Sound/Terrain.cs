using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Somnium
{
    public class Terrain : MonoBehaviour
    {
        /// <summary>
        /// Have all the possible sounds for the terrain loaded here.
        /// </summary>
        [SerializeField]
        private List<AudioClip> sounds;

        /// <summary>
        /// Play a random sound from our list
        /// </summary>
        public void PlaySound()
        {
            SoundManager.instance.PlaySingle(sounds[Random.Range(0, sounds.Count)]);
        }
    }
}
