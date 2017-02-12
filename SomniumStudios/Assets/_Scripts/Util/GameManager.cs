using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    [Tooltip("Screen fader controller")]
    public GameObject fade = new GameObject();


    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        //TODO don't look at me T_T
        fade = GameObject.Find("Fade");
    }

    public static void ChangeScene() {
        fade.GetComponent("ScreenFader").fadeIn = false;
        //TODO make this not hardcoded

        SceneManager.LoadScene(0);

    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
