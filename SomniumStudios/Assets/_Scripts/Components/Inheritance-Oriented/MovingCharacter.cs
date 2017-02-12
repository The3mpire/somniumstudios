using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class MovingCharacter : MonoBehaviour
    {

        //Character controllers only react to physics if moved. 
        //An OnCollision method would need to be implemented of physics acting upon this gameobject is desired.
        private CharacterController cController;

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
        private float speed = 1f;

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

        protected virtual void Awake()
        {
            this.cController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Deriving class will need to implement their own update, describing what inputs are used for Move and
        /// how the inputs are collected. Then it will need to call Move();
        /// </summary>
        protected abstract void Update();

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

            Vector2 vel = direction * speed;
            cController.Move(vel * Time.deltaTime);
        }
    }
}
