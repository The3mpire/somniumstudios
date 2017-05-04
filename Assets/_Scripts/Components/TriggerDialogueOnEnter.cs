using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    [RequireComponent(typeof(Collider))]
    public class TriggerDialogueOnEnter : MonoBehaviour
    {
        [SerializeField]
        private string dialogFilePath;

        [SerializeField]
        private Sprite profileSprite;

        [SerializeField]
        private bool repeatable;

        private bool triggered;

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter()
        {
            if(!triggered || (triggered && repeatable))
            {
                triggered = true;
                DialogManager.Instance.ProfileSprite = profileSprite;
                DialogManager.Instance.StartDialog(dialogFilePath);
            }
        }
    }
}
