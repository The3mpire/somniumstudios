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
            //Cursor.visible = true;
            Time.timeScale = 0;
        }
        else if (Input.GetButtonDown("Cancel") && menu) {
            pausePanel.SetActive(false);
            Cursor.visible = false;
            EventSystem.current.SetSelectedGameObject(null);
            menu = false;
            Time.timeScale = 1;
        }
    }


    public void ResumeGame() {
        pausePanel.SetActive(false);
        Cursor.visible = false;
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
    /// Continue from where you left off
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
