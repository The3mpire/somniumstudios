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


    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
