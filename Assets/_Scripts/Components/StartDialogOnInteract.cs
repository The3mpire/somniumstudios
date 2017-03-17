using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class StartDialogOnInteract : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string dialogFilePath;

        [SerializeField]
        private Sprite profileSprite;

        public void Interact()
        {
            Cursor.visible = true;

            // we've solved the puzzle, grab different dialog
            if (!StateMachine.instance.isUnsolved("StovenPuzzle")) {
                dialogFilePath += "Solved";
            }

            DialogManager.Instance.ProfileSprite = profileSprite;
            DialogManager.Instance.StartDialog(dialogFilePath);
        }
    }
}
