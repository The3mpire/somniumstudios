using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour {

    public Button startButton;
    public Button resumeButton;

	// Use this for initialization
	void Start () {
        if (resumeButton.interactable) {
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        else {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
