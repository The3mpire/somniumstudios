using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]

public class Draggable : MonoBehaviour {

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

    /// <summary>
    /// When you click the mouse and the item is draggable,
    /// the cursor hides and the object snaps to/follows the cursor
    /// </summary>
    void OnMouseDown() {
        if (isDraggable) {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            Cursor.visible = false;
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
        if (ReceptacleContains()/*receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0f))*/) {
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
        // TODO Play the sound effect
        SoundManager.instance.PlaySingle(sfx, 1f);

        // Make the object kinematic so it won't fall
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        Transform parentLocation = receptacle.transform;
        transform.position = parentLocation.position;

        //transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

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

        //TODO change to quaternion so player can rotate the piece freely && have a range of acceptable rotation (public variables)

        //check the x
        if (gameObject.transform.position.x <= (receptacle.transform.position.x + receptacle.GetComponent<BoxCollider>().size.x / 2)
             && gameObject.transform.position.x >= (receptacle.transform.position.x - receptacle.GetComponent<BoxCollider>().size.x / 2)
             && gameObject.transform.position.y <= (receptacle.transform.position.y + receptacle.GetComponent<BoxCollider>().size.y / 2)
             && gameObject.transform.position.y >= (receptacle.transform.position.y - receptacle.GetComponent<BoxCollider>().size.y / 2)) {
            return true;
        }

        // the object is not in the receptacle
        return false;
    }
}