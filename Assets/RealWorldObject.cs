using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Somnium {
    public class RealWorldObject : MonoBehaviour {



        // Use this for initialization
        void Start() {
            gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update() {

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
