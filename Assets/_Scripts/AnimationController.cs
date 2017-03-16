using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Somnium {

    public class AnimationController : MonoBehaviour, IInteractable {

        private Animator anim;

        public void Interact() {
            anim.SetBool("isShocked", true);
        }

        // Use this for initialization
        void Start() {
            anim = gameObject.GetComponent<Animator>();
            anim.SetBool("isShocked", false);
        }

        // Update is called once per frame
        void Update() {
        }
    }
}