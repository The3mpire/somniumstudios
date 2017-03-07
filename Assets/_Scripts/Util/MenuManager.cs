using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }


    /// <summary>
    /// Start the entire game over
    /// </summary>
    public void RestartGame() {
        GameManager.RestartGame();
    }

    /// <summary>
    /// Continue from where you left off
    /// </summary>
    public void ContinueGame() {
        GameManager.ContinueGame();
    }

    /// <summary>
    /// Go back to main menu
    /// </summary>
    public void ExitLevel() {
        GameManager.ExitLevel();
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    public void StartGame() {
        GameManager.StartGame();
    }

    //private IEnumerator menuWait(float time)
    // {
    //     yield return new WaitForSeconds(time);

    // }

    /// <summary>
    /// Exit the entire application
    /// </summary>
    public void ExitGame() {
        GameManager.ExitGame();
    }
}
