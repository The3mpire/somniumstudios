using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [Tooltip("The Pause gui")]
    public GameObject pausePanel;

    private bool menu = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (Input.GetButtonDown("Cancel") && !menu) {
            pausePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(pausePanel.GetComponentInChildren<Button>().gameObject);
            menu = true;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else if (Input.GetButtonDown("Cancel") && menu) {
            pausePanel.SetActive(false);

            // don't kill the mouse -- we're in a puzzle scene
            if (SceneManager.GetActiveScene().name.Contains("Puzzle")) {
                Cursor.visible = true;
            }
            // not a puzzle scene
            else {
                Cursor.visible = false;
            }

            EventSystem.current.SetSelectedGameObject(null);
            menu = false;
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Unpause the game
    /// </summary>
    public void ResumeGame() {
        pausePanel.SetActive(false);

        // don't kill the mouse -- we're in a puzzle scene
        if (SceneManager.GetActiveScene().name.Contains("Puzzle")) {
            Cursor.visible = true;
        }
        // not a puzzle scene
        else {
            Cursor.visible = false;
        }

        EventSystem.current.SetSelectedGameObject(null);
        menu = false;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Start the entire game over
    /// </summary>
    public void RestartGame() {
        GameManager.RestartGame();
    }

    /// <summary>
    /// BROKEN (continue from mainMenu)
    /// </summary>
    public void ContinueGame() {
        GameManager.ContinueGame();
    }

    /// <summary>
    /// Go back to main menu
    /// </summary>
    public void ExitLevel() {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
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
