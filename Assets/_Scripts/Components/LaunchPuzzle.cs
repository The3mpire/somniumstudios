using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Somnium {

    public sealed class LaunchPuzzle : MonoBehaviour {
        [Tooltip("Link the player so we can do shit with it")]
        [SerializeField]
        private GameObject player;

        private void OnEnable() {
            DialogManager.Instance.ChoiceSelectedEvent += Interact;
        }

        public void Interact(int choice, object val) {
            int.TryParse(val.ToString(), out choice);
            if (choice == 1 ) {
                StartCoroutine(WaitForDialog());
            }
        }

        private void OnDisable()
        {
            DialogManager.Instance.ChoiceSelectedEvent -= Interact;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        private IEnumerator WaitForDialog() {
            yield return new WaitWhile(() => (DialogManager.Instance.RunningDisplayRoutine));
            // set the previous location before we launch the puzzle
            GameManager.SetPreviousLocation(player.transform.position);
            GameManager.SetPreviousScene(SceneManager.GetActiveScene());
			StateMachine.wasInPuzzle = true;

            GameManager.ChangeScene(3);
        }
    }
}
