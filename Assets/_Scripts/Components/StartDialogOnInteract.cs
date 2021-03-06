﻿using System;
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

        [SerializeField]
        private bool repeatable;

        private bool triggered;

        public void Interact()
        {
            if(!triggered || (triggered && repeatable))
            {
                triggered = true;
                Cursor.visible = true;
                DialogManager.Instance.ProfileSprite = profileSprite;
                DialogManager.Instance.StartDialog(dialogFilePath);
            }
        }
    }
}
