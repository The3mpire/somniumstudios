using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Patrol : MonoBehaviour {

    [SerializeField]
    private Vector3 endPoint;

    [SerializeField]
    private Vector3 startPoint;

    private bool reached;

    // Use this for initialization
    void Start () {
        reached = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (this.transform.position.x > endPoint.x + .5 && !reached) {
            this.transform.DOMove(endPoint, 3);
            this.GetComponent<Animator>().SetBool("isWalking", true);
        }
        else if (reached) {
            this.GetComponent<Animator>().SetBool("isWalking", false);
        }
        else {
            reached = true;
        }
    }
}
