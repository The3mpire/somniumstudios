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

    public static void ChangeScene() {
        instance.fade.GetComponent<ScreenFader>().fadeIn = false;

        // coroutine before change scene
        instance.StartCoroutine("FadeTime");
        
    }

    private IEnumerator FadeTime() {
        yield return new WaitForSeconds(instance.fade.GetComponent<ScreenFader>().fadeTime);

        //TODO make this not hardcoded
        SceneManager.LoadSceneAsync(0);
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
