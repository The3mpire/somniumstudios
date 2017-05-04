using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour {

    [SerializeField]
    [Tooltip("Length of the axis to be checked (longer is more difficult for the player), 0 if you don't care about that axis")]
    private Vector3 directionCheck;

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
    [Tooltip("If this object has an object that must be placed before it can be placed")]
    private bool hasPredecessor = false;

    [SerializeField]
    [Tooltip("If this object is a predecessor")]
    private bool isPredecessor = false;

    [SerializeField]
    [Tooltip("If this goes into the puzzle")]
    private bool hasReceptacle = true;

    [SerializeField]
    [Tooltip("The receptacle object for this draggable object")]
    private GameObject receptacle;

    [SerializeField]
    [Tooltip("The receptacle object that this draggable depends on")]
    private GameObject predecessor;

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
    private float rotateSpeed = 8;

    /// <summary>
    /// Add the predecessor the puzzle manager
    /// </summary>
    void Awake() {
        if (isPredecessor) {
            PuzzleManager.setPredecessor(gameObject, false);
        }
    }

    /// <summary>
    /// Rotate
    /// </summary>
    void Update() {
        if (isClicked) {

            Vector3 input = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("Diagonal"));

            //rotate about the x
            if (input.x > 0) {
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

            WithinThreshold();
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

        // if their predecessor isn't placed, get outta that bitch
        if (hasPredecessor) {
            if (!PuzzleManager.isPredecessorPlaced(predecessor)) {
                return;
            }
        }

        /*receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0f))*/
        if (hasReceptacle && WithinThreshold()) {
            
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

        receptacle.GetComponent<BoxCollider>().enabled = false;

        PuzzleManager.incrementPiecesPlaced();

        if (isPredecessor) {
            PuzzleManager.setPredecessor(gameObject, true);
        }

        // Disable Renderers so the 3D objects don't show
        //gameObject.GetComponent<Renderer>().enabled = false;
        //gameObject.GetComponentInChildren<Renderer>().enabled = false;
        StartCoroutine(FadeIn());
        StartCoroutine(FadeOut());

    }

    ///// <summary>
    ///// Fade out the alpha channel of the draggable object
    ///// </summary>
    ///// <returns></returns>
    public IEnumerator FadeOut() {

        Renderer parentRend = GetComponent<Renderer>();
        if (parentRend)
            parentRend.enabled = false;
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        yield return null;
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
        gameObject.SetActive(false);

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

    ///// <summary>
    ///// Determines whether the object is within the receptacle x & y (DOES NOT CHECK Z)
    ///// </summary>
    ///// <returns></returns>
    ////private bool ReceptacleContains() {
    //     Debug.Log("In receptable contains");
    //    //check the x
    //    //if (gameObject.transform.position.x <= (receptacle.transform.position.x + receptacle.GetComponent<BoxCollider>().size.x / 2)
    //    //     && gameObject.transform.position.x >= (receptacle.transform.position.x - receptacle.GetComponent<BoxCollider>().size.x / 2)
    //    //     && gameObject.transform.position.y <= (receptacle.transform.position.y + receptacle.GetComponent<BoxCollider>().size.y / 2)
    //    //     && gameObject.transform.position.y >= (receptacle.transform.position.y - receptacle.GetComponent<BoxCollider>().size.y / 2)) {
    //        //is it within the threshold
    //        //receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(thresholdCollider.bounds.extents.x, thresholdCollider.bounds.extents.y, receptacle.GetComponent<BoxCollider>().bounds.extents.z))

    //        // figure out where the collider is facing, and how far it extends

    //       // return WithinThreshold();
    //  //  }

    //    // the object is not in the receptacle
    //    //return false;
    //}

    bool WithinThreshold() {
        bool within = true;

        if (directionCheck.x > 0) {
            // get the rotation of the object and then the length of the threshold check (in that direction)
            Vector3 offset = transform.right.normalized * directionCheck.x;

            //get the ends of the collider
            Vector3 posX = transform.position + offset;
            Vector3 negX = transform.position - offset;

            Debug.DrawLine(transform.position, posX, Color.red);
            Debug.DrawLine(transform.position, negX, Color.red);
            if (!(receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(receptacle.transform.position.x, posX.y, posX.z))
                && receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(receptacle.transform.position.x, negX.y, negX.z)))) {
                return false;
            }
        }
        if (within && directionCheck.y > 0) {
            // get the rotation of the object and then the length of the threshold check (in that direction)
            Vector3 offset = transform.up.normalized * directionCheck.y;

            //get the ends of the collider
            Vector3 posY = transform.position + offset;
            Vector3 negY = transform.position - offset;

            Debug.DrawLine(transform.position, negY, Color.green);
            Debug.DrawLine(transform.position, posY, Color.green);
            if (!(receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(posY.x, receptacle.transform.position.y, posY.z))
                && receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(posY.x, receptacle.transform.position.y, negY.z)))) {
                return false;
            }
        }
        if (within && directionCheck.z > 0) {
            // get the rotation of the object and then the length of the threshold check (in that direction)
            Vector3 offset = transform.forward.normalized * directionCheck.z;

            //get the ends of the collider
            Vector3 posZ = transform.position + offset;
            Vector3 negZ = transform.position - offset;

            Debug.DrawLine(transform.position, negZ, Color.blue);
            Debug.DrawLine(transform.position, posZ, Color.blue);
            if (!(receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(posZ.x, posZ.y, receptacle.transform.position.z))
                && receptacle.GetComponent<BoxCollider>().bounds.Contains(new Vector3(posZ.x, negZ.y, receptacle.transform.position.z)))) {
                return false;
            }
        }

        return within;
    }
}