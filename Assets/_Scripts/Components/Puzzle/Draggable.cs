﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]

public class Draggable : MonoBehaviour {

	[SerializeField]
	private BoxCollider thresholdCollider;

    [SerializeField]
    [Tooltip("Sound effect for the draggable object")]
    private AudioClip sfx;

    private Vector3 screenPoint;
    //private Vector3 offset;
    [SerializeField]
    [Tooltip("If this object is draggable")]
    private bool isDraggable = true;

    [SerializeField]
    [Tooltip("If this object has an object in its way")]
    private bool hasInterruptor = false;

    [SerializeField]
    [Tooltip("The receptacle object for this draggable object")]
    private GameObject receptacle;

    [SerializeField]
    [Tooltip("The interruptor object if this draggable has one")]
    private GameObject interruptor;

    [SerializeField]
    [Tooltip("The time it takes for the object to fade in/out")]
    private float fadeTime;

    private Color receptacleColor;
    private Color itemColor;

    private bool isClicked;

    [SerializeField]
    private bool hasReceptacle;

    [SerializeField]
    private float rotateSpeed = 10;

    /// <summary>
    /// Rotate
    /// </summary>
    void Update() {
        if (isClicked) {

            Vector3 input = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("Diagonal"));

            //rotate about the x
			if (input.x > 0){
                transform.Rotate(new Vector3(1, 0, 0), rotateSpeed);
            }
            else if (input.x < 0) {
                transform.Rotate(new Vector3(1, 0, 0), -rotateSpeed);
            }

            //rotate about the y
            if (input.y > 0) {
                transform.Rotate(new Vector3(0, 1, 0), rotateSpeed);
            }
            else if (input.y < 0) {
                transform.Rotate(new Vector3(0, 1, 0), -rotateSpeed);
            }

            //rotate about the z
            if (input.z > 0) {
                transform.Rotate(new Vector3(0, 0, 1), rotateSpeed);
            }
            else if (input.z < 0) {
                transform.Rotate(new Vector3(0, 0, 1), -rotateSpeed);
            }
      	}
    }


    /// <summary>
    /// When you click the mouse and the item is draggable,
    /// the cursor hides and the object snaps to/follows the cursor
    /// </summary>
    void OnMouseDown() {
        if (isDraggable) {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            Cursor.visible = false;

            isClicked = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    /// <summary>
    /// When dragging with the mouse on an object, if follows the mouse position
    /// </summary>
    void OnMouseDrag() {
        if (isDraggable) {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            transform.position = curPosition;
        }
    }

    /// <summary>
    /// When the mouse is released, check the position of the object and see if it is in the correct location
    /// to snap to in the puzzle.
    /// </summary>
    void OnMouseUp() {
        Cursor.visible = true;

        isClicked = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;

        if (hasReceptacle && ReceptacleContains()/*receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0f))*/) {
            if (hasInterruptor) {
                if (interruptor.GetComponent<Interruptor>().getLight())
                    Snap();
            }
            else
                Snap();
        }

    }

    /// <summary>
    /// Helper method to snap the object in place
    /// </summary>
    void Snap() {
        //  Play the sound effect
        SoundManager.instance.PlaySingle(sfx, 1f);

        // Make the object kinematic so it won't fall
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        Transform parentLocation = receptacle.transform;
        transform.position = parentLocation.position;

        isDraggable = false;

        if (hasInterruptor)
            interruptor.GetComponent<Interruptor>().setClickable(false);

        // Disable the colliders so it is no longer clickable
        GetComponent<MeshCollider>().enabled = false;
        receptacle.GetComponent<BoxCollider>().enabled = false;

        PuzzleManager.incrementPiecesPlaced();

        // Disable Renderers so the 3D objects don't show
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;

        //StartCoroutine(FadeOut());
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fade out the alpha channel of the draggable object
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOut() {
        itemColor = gameObject.GetComponent<MeshRenderer>().material.color;
        while (itemColor.a > 0) {
            itemColor.a -= 0.01f;
            gameObject.GetComponent<MeshRenderer>().material.color = itemColor;
            yield return new WaitForSeconds(fadeTime);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Fade in the alpha channel of the 2D sprite of the 3D object in the puzzle
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeIn() {
        receptacleColor = receptacle.GetComponent<SpriteRenderer>().color;
        while (receptacleColor.a < 1) {
            receptacleColor.a += 0.01f;
            receptacle.GetComponent<SpriteRenderer>().color = receptacleColor;
            yield return new WaitForSeconds(fadeTime);
        }
        StartCoroutine(Wait(0.001f));
    }
	//TODO just add another collider my dude

    /// <summary>
    /// Helper coroutine to wait a certain amount of time
    /// </summary>
    /// <param name="time"> time to wait </param>
    /// <returns></returns>
    public IEnumerator Wait(float time) {
        yield return new WaitForSecondsRealtime(time);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Determines whether the object is within the receptacle x & y (DOES NOT CHECK Z)
    /// </summary>
    /// <returns></returns>
    private bool ReceptacleContains() {
       // Debug.Log("In receptable contains");
        //check the x
        if (gameObject.transform.position.x <= (receptacle.transform.position.x + receptacle.GetComponent<BoxCollider>().size.x / 2)
             && gameObject.transform.position.x >= (receptacle.transform.position.x - receptacle.GetComponent<BoxCollider>().size.x / 2)
             && gameObject.transform.position.y <= (receptacle.transform.position.y + receptacle.GetComponent<BoxCollider>().size.y / 2)
             && gameObject.transform.position.y >= (receptacle.transform.position.y - receptacle.GetComponent<BoxCollider>().size.y / 2)) {
            //is it within the threshold
			//receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(thresholdCollider.bounds.extents.x, thresholdCollider.bounds.extents.y, receptacle.GetComponent<BoxCollider>().bounds.extents.z))
			return thresholdCollider.bounds.Intersects(receptacle.GetComponent<BoxCollider> ().bounds);
        }

        // the object is not in the receptacle
        return false;
    }
}