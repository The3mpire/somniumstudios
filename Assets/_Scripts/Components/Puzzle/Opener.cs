using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opener : MonoBehaviour {

    private bool light = false;
    private bool clickable = true;
    private Color32 origColor;

    // Use this for initialization
    void Start() {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
    }

    // Update is called once per frame
    void Update() {

    }

    void OnMouseDown() {
        if (clickable) {
            if (light) {
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                light = false;
            }
            else {
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0x5C);
                light = true;
            }
        }
    }

    public bool getLight() {
        return light;
    }

    public void setClickable(bool c) {
        clickable = c;
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
    }

}

