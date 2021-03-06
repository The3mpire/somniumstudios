﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interruptor : MonoBehaviour {

    private bool light = false;
    private bool clickable = true;

    [SerializeField]
    private bool isOpener = false;

	// Use this for initialization
	void Start () {
        if (isOpener) {
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0x00);
        }
        else {
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnMouseDown() {
        if (clickable && !isOpener) {
            if (light) {
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                light = false;
            }
            else {
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0x5C);
                light = true;
            }
        }
        else if(clickable && isOpener) {
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            light = true;
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
