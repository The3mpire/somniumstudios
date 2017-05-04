using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class StartDialogueOnSceneLoad : MonoBehaviour
    {

        [SerializeField]
        private string dialogueFilePath;

        [SerializeField]
        private Sprite profileSprite;

        [SerializeField]
        private string flag;

        [SerializeField]
        private int flagValue;

        private void OnLevelWasLoaded()
        {
            if (StateMachine.instance.GetFlag(flag)==flagValue)
            {
                DialogManager.Instance.ProfileSprite = profileSprite;
                DialogManager.Instance.StartDialog(dialogueFilePath);
            }
        }
    }
}
