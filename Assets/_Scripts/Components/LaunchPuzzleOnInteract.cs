using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    [RequireComponent(typeof(LaunchPuzzle))]
    public class LaunchPuzzleOnInteract : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            this.gameObject.GetComponent<LaunchPuzzle>().Interact(1, 1);
        }

    }
}
