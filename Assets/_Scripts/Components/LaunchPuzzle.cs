using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium {

    public sealed class LaunchPuzzle : MonoBehaviour {

        private void Start() {
            DialogManager.Instance.ChoiceSelectedEvent += Interact;
        }


        public void Interact(int choice, object val) {
            int.TryParse(val.ToString(), out choice);
            if (choice == 1 ) {
                StartCoroutine(WaitForDialog());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        private IEnumerator WaitForDialog() {
            yield return new WaitWhile(() => (DialogManager.Instance.runningDisplayRoutine));
            GameManager.ChangeScene(3);

        }
    }
}
