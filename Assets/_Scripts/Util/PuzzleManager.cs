using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour {
    [Tooltip("don't touch this")]
    public static PuzzleManager instance = null;
    [Tooltip("How many pieces this puzzle has")]
    public int pieceCount = 2;
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

        piecesPlaced = 0;
        Cursor.visible = true;
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

        if(piecesPlaced == instance.pieceCount) {
            //TODO make not hardcoded
            GameManager.ChangeScene(2);
        }
    }

}
