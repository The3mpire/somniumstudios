using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
   
    public sealed class LaunchPuzzle : MonoBehaviour, IInteractable { 

        private void Start()
        {
        }

        public void Interact()
        {
            Debug.Log("here");
            GameManager.ChangeScene(1);
        }
    }
}
