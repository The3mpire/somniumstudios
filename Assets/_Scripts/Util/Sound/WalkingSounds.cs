using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Somnium {
    public class WalkingSounds : MonoBehaviour {

        [SerializeField]
        private List<AudioClip> defaultSounds;


        /// <summary>
        /// Plays the a random sound based on what the floor is
        /// </summary>
        void PlayWalkingSounds() {
            //either play the sound of the terrain below
            RaycastHit hit;


            if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 8)) {
                hit.collider.gameObject.GetComponent<Terrain>().PlaySound();
            }

            //TODO or play a default sound if there is no specified terrain below
        }
    }
}
