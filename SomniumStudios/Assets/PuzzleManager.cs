﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour {
    [Tooltip("don't touch this")]
    public static PuzzleManager instance = null;
    [Tooltip("How many pieces this puzzle has")]
    public static int pieceCount = 2;
    [Tooltip("Screen fader controller")]
    public ScreenFader fade;

    [SerializeField]
    private static int piecesPlaced;

    [SerializeField]
    private static int layerCounter;

    // Use this for initialization
    void Awake() {
        //make sure we don't havae more than 2 instances
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update() {
        if(pieceCount == piecesPlaced) {
            GameManager.ChangeScene();
        }
    }

    

    /// <summary>
    /// Return the layer counter and increment
    /// </summary>
    /// <returns></returns>
    public static int getLayerCounter() {
        return ++layerCounter;
    }

    public static void incrementPiecesPlaced() {
        piecesPlaced++;
    }

}
