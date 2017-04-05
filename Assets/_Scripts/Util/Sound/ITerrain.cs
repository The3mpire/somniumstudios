using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public interface ITerrain
    {

        void PlaySound(TerrainSound soundType);

        void PlayFootstep();
    }

    public enum TerrainSound
    {
        FOOTSTEP
    }
}
