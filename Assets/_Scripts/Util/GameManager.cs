using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    [Tooltip("Screen fader controller")]
    public GameObject fade;


    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
    }


    #region Level/Scene Methods

    public static void ChangeScene(int levelIndex) {
        instance.fade.GetComponent<ScreenFader>().fadeIn = false;

        // coroutine before change scene
        IEnumerator coroutine = instance.FadeTime(levelIndex);
        instance.StartCoroutine(coroutine);
    }

    private IEnumerator FadeTime(int levelIndex) {
        yield return new WaitForSeconds(instance.fade.GetComponent<ScreenFader>().fadeTime);
     
        SceneManager.LoadSceneAsync(levelIndex);
    }

    void OnLevelWasLoaded() {
        instance.fade.GetComponent<ScreenFader>().fadeIn = true;
    }

    /// <summary>
    /// Restart from most recent checkpoint/levelLoad
    /// </summary>
    public void RestartLevel() {
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /// <summary>
    /// Start the entire game over
    /// </summary>
    public void RestartGame() {
        Cursor.visible = false;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Continue from where you left off
    /// </summary>
    public void ContinueGame() {
        Cursor.visible = false;
        SceneManager.LoadScene(PlayerPrefs.GetInt("level"));
    }

    /// <summary>
    /// Go back to main menu
    /// </summary>
    public void ExitLevel() {
        Cursor.visible = false;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Start a brand new game
    /// </summary>
    public void NewGame() {
        PlayerPrefs.SetInt("points", 0);
        PlayerPrefs.SetInt("level", 0);
        Cursor.visible = false;
        SceneManager.LoadScene(1);
    }

    //private IEnumerator menuWait(float time)
    // {
    //     yield return new WaitForSeconds(time);

    // }

    /// <summary>
    /// Exit the entire application
    /// </summary>
    public void ExitGame() {
        Application.Quit();
    }
    #endregion
}
