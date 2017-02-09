using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{

    public sealed class Interactor : MonoBehaviour
    {
        /// <summary>
        /// Minimum interaction distance
        /// </summary>
        [SerializeField]
        private float interactDistance = 5f;

        /// <summary>
        /// Layers that this object can interact with.
        /// </summary>
        [SerializeField]
        private LayerMask interactableLayer;

        /// <summary>
        /// Interacts with the object in the specified direction if it is close enough and on the interactable layers.
        /// </summary>
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
