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
    [Tooltip("How many pieces are placed")]
    private static int piecesPlaced;

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
    /// Increment the number of pieces placed and if all of the pieces
    /// have been placed then change back to the original scene and out of the puzzle
    /// </summary>
    public static void incrementPiecesPlaced() {
        piecesPlaced++;

        if(piecesPlaced == instance.pieceCount) {

            StateMachine.instance.updateMemDictionary(SceneManager.GetActiveScene().name, false);
            //TODO make not hardcoded
            GameManager.ChangeScene(2);
        }
    }

}
