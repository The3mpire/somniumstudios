using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]

public class Draggable : MonoBehaviour {

    [SerializeField]
    private AudioClip sfx;

    private Vector3 screenPoint;
    //private Vector3 offset;
    [SerializeField]
    private bool isDraggable = true;

    [SerializeField]
    private bool hasInterruptor = false;

    [SerializeField]
    private GameObject receptacle;

    [SerializeField]
    private GameObject interruptor;

    [SerializeField]
    private float fadeTime;

    private Color receptacleColor;
    private Color itemColor;

    //TODO put in puzzle manager
    private int layerCount = 0;

    void OnMouseDown()
    {
        if (isDraggable)
        {
           // gameObject.GetComponent<SpriteRenderer>().sortingOrder = PuzzleManager.getLayerCounter();

            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            //offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
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
        if (receptacle.GetComponent<BoxCollider2D>().bounds.Contains(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0f)));
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

        SoundManager.instance.PlaySingle(sfx, 1f);


        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        Transform parentLocation = receptacle.transform;

        transform.position = parentLocation.position;

        //transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

      //  GetComponent<SpriteRenderer>().sortingOrder = -1;

        isDraggable = false;

        if (hasInterruptor)
            interruptor.GetComponent<Interruptor>().setClickable(false);

        GetComponent<MeshCollider>().enabled = false;
        receptacle.GetComponent<BoxCollider2D>().enabled = false;

        PuzzleManager.incrementPiecesPlaced();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        //StartCoroutine(FadeOut());
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        itemColor = gameObject.GetComponent<MeshRenderer>().material.color;
        while (itemColor.a > 0)
        {
            itemColor.a -= 0.01f;
            gameObject.GetComponent<MeshRenderer>().material.color = itemColor;
            yield return new WaitForSeconds(fadeTime);
        }
        gameObject.SetActive(false);
    }

    public IEnumerator FadeIn()
    {
        receptacleColor = receptacle.GetComponent<SpriteRenderer>().color;
        while (receptacleColor.a < 1)
        {
            receptacleColor.a += 0.01f;
            receptacle.GetComponent<SpriteRenderer>().color = receptacleColor;
            yield return new WaitForSeconds(fadeTime);
        }
        StartCoroutine(Wait(0.5f));
    }
    public IEnumerator Wait(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        gameObject.SetActive(false);
    }
}