using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class TriggerDialogue : MonoBehaviour
    {
        [SerializeField]
        private bool beforePuzzleSolve;

        [SerializeField]
        private string puzzleName;

        [SerializeField]
        private string dialogFilePath;

        [SerializeField]
        private Sprite profileSprite;

        [SerializeField]
        private bool Repeatable;

        void OnTriggerEnter()
        {
            // if before puzzle deactivate the object when the puzzle is solved
            if (beforePuzzleSolve)
            {
                if (!StateMachine.instance.isUnsolved(puzzleName))
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
            else
            {
                if (!StateMachine.instance.isUnsolved(puzzleName))
                {
                    DialogManager.Instance.ProfileSprite = profileSprite;
                    DialogManager.Instance.StartDialog(dialogFilePath);

                    if (gameObject.GetComponentInParent<NPCMovement>() != null)
                    {
                        StartCoroutine(Wait());
                    }
                    else
                    {
                        this.gameObject.SetActive(Repeatable);
                    }
                }
            }
        }

        private IEnumerator Wait()
        {
            yield return new WaitWhile(()=>{ return DialogManager.Instance.RunningDisplayRoutine; });
                        gameObject.GetComponentInParent<NPCMovement>().MoveCharacter();
        }
    }
}