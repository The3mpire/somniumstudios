using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    [Tooltip("Screen fader controller")]
    public GameObject fade;

    // where the player was standing when the puzzle launched
    private static Vector3 previousLocation;
    private static int previousSceneIndex;

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
        // TODO fix this awful hardcoded bullshit fuck you Karla you stupid bitch
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
    /// Change the scene to the given scene - using fading
    /// </summary>
    /// <param name="levelIndex"> the index in the build of the scene to change to </param>
    public static void ChangeScene(Scene scene) {
        // just call the other method

        ChangeScene(scene.buildIndex);
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

    #region Puzzle Related Methods

    /// <summary>
    /// Set the previous location of the player 
    /// </summary>
    /// <param name="playerLocation"></param>
    public static void SetPreviousLocation(Vector3 playerLocation) {
        previousLocation = playerLocation;
    }

    /// <summary>
    /// Get the previous location of the player
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetPreviousLocation() {
        return previousLocation;
    }

    /// <summary>
    /// Set the previous scene the player was in 
    /// </summary>
    /// <param name="prevScene"></param>
    public static void SetPreviousScene(Scene prevScene) {
        previousSceneIndex = prevScene.buildIndex;
    }

    /// <summary>
    /// Get the previous Scene the player was in
    /// </summary>
    /// <returns></returns>
    public static int GetPreviousScene() {
        return previousSceneIndex;
    }

    #endregion

}
