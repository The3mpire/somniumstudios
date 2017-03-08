using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Draggable : MonoBehaviour {
    private Vector3 screenPoint;
    //private Vector3 offset;
    public bool isDraggable = true;
    public bool hasInterruptor = false;
    public GameObject receptacle;
    public GameObject interruptor;

    //TODO put in puzzle manager
    private int layerCount = 0;

    void OnMouseDown()
    {
        if (isDraggable)
        {
           // gameObject.GetComponent<SpriteRenderer>().sortingOrder = PuzzleManager.getLayerCounter();

            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

           // offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            Cursor.visible = false;
        }
    }

    void OnMouseDrag()
    {
        if (isDraggable)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            transform.position = curPosition;
        }
    }

    void OnMouseUp()
    {
        if (receptacle.GetComponent<BoxCollider2D>().bounds.Contains(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000f))))
        {
            if (hasInterruptor)
            {
                if (interruptor.GetComponent<Interruptor>().getLight())
                    Snap();
            }
            else
                Snap();
        }
        Cursor.visible = true;
    }

    void Snap()
    {
        Transform parentLocation = receptacle.transform;

        transform.position = parentLocation.position;

        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

      //  GetComponent<SpriteRenderer>().sortingOrder = -1;

        isDraggable = false;

        if (hasInterruptor)
            interruptor.GetComponent<Interruptor>().setClickable(false);

        GetComponent<BoxCollider2D>().enabled = false;
        receptacle.GetComponent<BoxCollider2D>().enabled = false;

        PuzzleManager.incrementPiecesPlaced();
    }
}