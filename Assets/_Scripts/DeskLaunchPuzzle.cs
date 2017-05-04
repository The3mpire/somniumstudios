using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium {
    public class DeskLaunchPuzzle : MonoBehaviour, IInteractable {
        public void Interact() {
            GameManager.ChangeScene(5);
        }
    }
}
