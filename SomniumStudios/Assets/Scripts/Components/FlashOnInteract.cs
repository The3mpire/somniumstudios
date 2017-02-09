using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class FlashOnInteract : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// The objects sprite renderer
        /// </summary>
        private SpriteRenderer render;

        /// <summary>
        /// Whether or not the coroutine is running.
        /// </summary>
        private bool isRunning;

        private void Start()
        {
            render = GetComponent<SpriteRenderer>();
        }

        public void Interact()
        {
            if (!isRunning)
            {
                StartCoroutine(FlashColor(4, .25f, Color.red));
            }
        }

        IEnumerator FlashColor(int flashCount, float interval, Color color)
        {
            isRunning = true;
            Color original = render.color;
            for(int i=0; i < flashCount; i++)
            {
                render.color = color;
                yield return new WaitForSeconds(interval);
                render.color = original;
                yield return new WaitForSeconds(interval);
            }
            isRunning = false;
        }
    }
}
