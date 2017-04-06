using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Somnium{
public class ObjectWaitDialog : MonoBehaviour {

	[SerializeField]
	private Sprite dialogueSprite;

	[SerializeField]
	private string dialogFile;


	[SerializeField]
	private float waitTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnLevelWasLoaded() {
		StartCoroutine(waitToTalk (waitTime));
	}

	private IEnumerator waitToTalk(float time){
		yield return new WaitForSeconds (time);


			DialogManager.Instance.ProfileSprite = dialogueSprite;
			DialogManager.Instance.StartDialog (dialogFile);

	}
}
}
