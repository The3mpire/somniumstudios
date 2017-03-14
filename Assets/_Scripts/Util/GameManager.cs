using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

    /// <summary>
    /// Change the scene to the given scene number - using fading
    /// </summary>
    /// <param name="levelIndex"> the index in the build of the scene to change to </param>
    public static void ChangeScene(int levelIndex) {
        // On puzzles and the main menu, dont hid the cursor
        if (levelIndex == 3 || levelIndex == 0)
        {
            Cursor.visible = true;
        }
        instance.fade.GetComponent<ScreenFader>().fadeIn = false;

        // coroutine before change scene
        IEnumerator coroutine = instance.FadeTime(levelIndex);
        instance.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Helper method to fade in and out of scenes
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    private IEnumerator FadeTime(int levelIndex) {
        yield return new WaitForSeconds(instance.fade.GetComponent<ScreenFader>().fadeTime);
     
        SceneManager.LoadSceneAsync(levelIndex);
    }

    /// <summary>
    /// Fade in the level when it loads
    /// </summary>
    void OnLevelWasLoaded() {
        instance.fade.GetComponent<ScreenFader>().fadeIn = true;
    }

    /// <summary>
    /// When the start button is pressed on the main menu, load 1st scene
    /// </summary>
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
        EventSystem.current.SetSelectedGameObject(null);
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
