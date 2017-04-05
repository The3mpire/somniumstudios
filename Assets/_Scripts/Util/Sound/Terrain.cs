using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class Terrain : ITerrain
    {
        /// <summary>
        /// Have all the possible sounds for the terrain loaded here.
        /// </summary>
        [SerializeField]
        private List<AudioClip> sounds;

        /// <summary>
        /// Can either have a method per sound or...
        /// </summary>
        public void PlayFootstep()
        {
            SoundManager.instance.PlaySingle(sounds[0]);
        }

        /// <summary>
        /// ... Have a single method that specifies the sound type, ie Footstep, running, walking, splashing, etc.
        /// </summary>
        /// <param name="soundType"></param>
        public void PlaySound(TerrainSound soundType)
        {
            switch (soundType)
            {
                case TerrainSound.FOOTSTEP:
                    SoundManager.instance.PlaySingle(sounds[0]);
                    break;
            }
        }
    }
}
