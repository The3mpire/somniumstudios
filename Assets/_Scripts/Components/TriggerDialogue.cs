using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class TriggerDialogue : MonoBehaviour
    {
        [SerializeField]
        private string dialogFilePath;

        [SerializeField]
        private Sprite profileSprite;

        [SerializeField]
        private bool Repeatable;

        void OnTriggerEnter()
        {
            if (!StateMachine.instance.isUnsolved("StovenPuzzle"))
            {
                gameObject.SetActive(false);
            }
            else
            {
                DialogManager.Instance.ProfileSprite = profileSprite;
                DialogManager.Instance.StartDialog(dialogFilePath);
                this.gameObject.SetActive(Repeatable);

            }
        }
    }
}