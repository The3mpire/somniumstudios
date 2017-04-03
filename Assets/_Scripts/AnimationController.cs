using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Somnium {

    public class AnimationController : MonoBehaviour, IInteractable {
      //[SerializeField]
        private Animator anim;
        private int seen;

        public void Interact() {
            if (seen == 1) {
                anim.SetBool("isShocked", true);
				gameObject.GetComponent<SpriteRenderer> ().flipX = true;
            }
            else
                anim.SetBool("isShocked", false);
            seen++;
        }

        // Use this for initialization
        void Start() {
            seen = 0;
            DialogManager.Instance.NewPageEvent += Interact;
            anim = gameObject.GetComponent<Animator>();
            anim.SetBool("isShocked", false);
			gameObject.GetComponent<SpriteRenderer> ().flipX = false;
        }
    }
}