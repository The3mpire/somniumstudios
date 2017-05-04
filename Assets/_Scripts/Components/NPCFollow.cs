using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollow : MonoBehaviour {

	[Tooltip ("The object that this NPC will follow")]
	[SerializeField]
	private GameObject followObj;

	[Tooltip ("The rotation speed of this NPC")]
	[SerializeField]
	private int rotSpeed;

	[Tooltip ("The movement speed of this NPC")]
	[SerializeField]
	private int moveSpeed;

	private SpriteRenderer sr;
	private Transform target;
	private int xSpeed;
	private int zSpeed;
	private float buffer;

    public GameObject FollowObj
    {
        get
        {
            return followObj;
        }
        set
        {
            this.target = value.transform;
            this.followObj = value;
        }
    }

	// Use this for initialization
	void Start () {
        if(followObj)
		    target = followObj.GetComponent<Transform> ();
		sr = gameObject.GetComponent<SpriteRenderer> ();

		xSpeed = moveSpeed;
		zSpeed = moveSpeed;

		buffer = 1f;
	}
	
	// Update is called once per frame
	void Update () 
	{
        // fully rotate
        //		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation,
        //			Quaternion.LookRotation(target.position - gameObject.transform.position), rotSpeed*Time.deltaTime);

        //move towards the player
        //		gameObject.transform.position += gameObject.transform.forward * moveSpeed * Time.deltaTime;

        // face the player
        if (followObj) {
            //move towards the player
            if (Vector3.Distance(transform.position, target.position) > 2f) {
                //move if distance from target is greater than 1

                if ((transform.position.x > target.position.x + buffer) || (transform.position.x < target.position.x - buffer)) {
                    if ((xSpeed > 0) && transform.position.x > target.position.x) {
                        xSpeed *= -1;
                        sr.flipX = false;
                    }
                    else if ((xSpeed < 0) && transform.position.x < target.position.x) {
                        xSpeed *= -1;
                        sr.flipX = true;
                    }
                    transform.position += transform.right * xSpeed * Time.deltaTime;
                }

                if ((transform.position.z > target.position.z + buffer) || (transform.position.z < target.position.z - buffer)) {
                    if ((zSpeed > 0) && transform.position.z > target.position.z) {
                        zSpeed *= -1;
                    }
                    else if ((zSpeed < 0) && transform.position.z < target.position.z) {
                        zSpeed *= -1;
                    }
                    transform.position += transform.forward * zSpeed * Time.deltaTime;
                }
            }
        }

	}
}
