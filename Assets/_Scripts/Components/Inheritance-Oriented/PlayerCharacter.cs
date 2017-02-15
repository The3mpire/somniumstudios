using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class PlayerCharacter : MovingCharacter
    {
        /// <summary>
        /// Minimum distance of interaction.
        /// </summary>
        [SerializeField]
        private float interactDistance;

        /// <summary>
        /// Interactable layers.
        /// </summary>
        [SerializeField]
        private LayerMask interactableLayer;

        /// <summary>
        /// Last direction
        /// </summary>
        private Vector2 savedDirection;

        protected override void Update()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            savedDirection = input == Vector2.zero ? savedDirection : input;
            Move(input);

            if (Input.GetButtonDown("Interact"))
            {
                Interact(savedDirection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inDirection"></param>
        public void Interact(Vector3 inDirection)
        {
            inDirection = inDirection.normalized;
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, inDirection, interactDistance, interactableLayer);
            Debug.DrawRay(transform.position, inDirection, Color.green);
            if (hit)
            {
                IInteractable inter = hit.collider.gameObject.GetComponent<IInteractable>();
                if (inter != null)
                {
                    inter.Interact();
                }
            }
        }
    }
}
