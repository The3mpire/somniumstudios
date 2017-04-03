using System;
using System.Collections;
using UnityEngine;

namespace Somnium
{
    /// <summary>
    /// This class is responsible giving a gameobject movement abilities.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class Movement : MonoBehaviour
    {
        //Rigidbodies always react to physics.
        private CharacterController rigidBody;

        private Collider collider;

        public float gravity = 20.0F;

        /// <summary>
        /// Collision avoidance settings for gameobject.
        /// </summary>
        [SerializeField]
        private AvoidanceSettings avoidanceSettings;

        /// <summary>
        /// Collision avoidance settings struct
        /// </summary>
        [Serializable]
        private struct AvoidanceSettings
        {
            public bool useCollisionAvoidance;
            public LayerMask layersToAvoid;
            public float distance;
        }

        /// <summary>
        /// Speed of the gameobject 
        /// </summary>
        [SerializeField]
        private float speed = 5f;

        /// <summary>
        /// Speed of the gameobject 
        /// </summary>
        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (value > 0)
                {
                    this.speed = value;
                }
            }
        }

        void Start()
        {
            this.rigidBody = GetComponent<CharacterController>();
            this.collider = GetComponent<Collider>();
        }

        /// <summary>
        /// Moves this gameobject in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move in.</param>
        public void Move(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                return;
            }

            if (avoidanceSettings.useCollisionAvoidance)
            {
                MovementUtils.AvoidCollision(transform.position, ref direction, avoidanceSettings.distance, avoidanceSettings.layersToAvoid);
            }

            direction = direction.normalized;


            direction = transform.TransformDirection(direction);
            direction *= speed;

            rigidBody.Move(direction * Time.deltaTime);
            if (!rigidBody.isGrounded)
            {
                StartCoroutine(Fall(direction));

            }
            //rigidBody.MovePosition(rigidBody.position + (vel * Time.deltaTime));
        }

        private IEnumerator Fall(Vector3 direction)
        {
            while (!rigidBody.isGrounded)
            {
                rigidBody.Move(new Vector3(0, -gravity, 0) * Time.deltaTime);
                yield return null;
            }
        }
    }
}