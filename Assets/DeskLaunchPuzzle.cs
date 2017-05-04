using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Somnium 
{
    public class DeskLaunnchPuzzle : MonoBehaviour, IInteractable {
        public void Interact() {

            //GameManager.SetPreviousLocation(player.transform.position);
            GameManager.SetPreviousScene(SceneManager.GetSceneAt(6));
            this.gameObject.GetComponent<LaunchPuzzle>().Interact(0, 1);
        }

    }
}
