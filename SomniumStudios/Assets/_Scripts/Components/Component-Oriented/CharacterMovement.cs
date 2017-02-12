using System;
using UnityEngine;

namespace Somnium
{
    /// <summary>
    /// This class is responsible giving a gameobject movement abilities.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class CharacterMovement : MonoBehaviour
    {
        //Rigidbodies always react to physics.
        private Rigidbody2D rigidBody;

        /// <summary>
        /// Collision avoidance settings for gameobject.
        /// </summary>
        [SerializeField]
        private AvoidanceSettings avoidanceSettings;

        /// <summary>
        /// Collision avoidance settings struct
        /// </summary>
        [Serializable]
        private struct AvoidanceSettings{
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
            this.rigidBody = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Moves this gameobject in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move in.</param>
        public void Move(Vector3 direction)
        {
            if(direction == Vector3.zero)
            {
                return;
            }

            if (avoidanceSettings.useCollisionAvoidance)
            {
                MovementUtils.AvoidCollision(transform.position, ref direction, avoidanceSettings.distance, avoidanceSettings.layersToAvoid);
            }

            direction = direction.normalized;

            Vector2 vel = direction * speed;
            rigidBody.MovePosition(rigidBody.position + (vel * Time.deltaTime));
        }
    }
}