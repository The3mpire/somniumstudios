using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Somnium {
    public class RealWorldObject : MonoBehaviour {

        void Start() {
            gameObject.SetActive(true);
        }

        void OnLevelWasLoaded() {
            if (StateMachine.instance.isUnsolved(gameObject.name + "Puzzle")) {
                gameObject.SetActive(true);
            }
            else
                gameObject.SetActive(false);
        }
    }
}
