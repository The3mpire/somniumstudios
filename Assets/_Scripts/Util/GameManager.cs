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


    public static void StartGame() {
        Cursor.visible = false;
        ChangeScene(1);
    }

    /// <summary>
    /// Start the entire game over
    /// </summary>
    public static void RestartGame() {
        Cursor.visible = false;
        ChangeScene(1);
    }

    /// <summary>
    /// Continue from where you left off
    /// </summary>
    public static void ContinueGame() {
        Cursor.visible = false;
        //TODO do the thing
    }

    /// <summary>
    /// Go back to main menu
    /// </summary>
    public static void ExitLevel() {
        Cursor.visible = false;
        ChangeScene(0);
    }

    //private IEnumerator menuWait(float time)
    // {
    //     yield return new WaitForSeconds(time);

    // }

    /// <summary>
    /// Exit the entire application
    /// </summary>
    public static void ExitGame() {
        Application.Quit();
    }

    #endregion
}
