using UnityEngine;

namespace Somnium
{
    /// <summary>
    /// Attach to a camera object to enable target following.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFollow : MonoBehaviour
    {

        [Tooltip("How far above the player the camera will be")]
        [SerializeField]
        private float verticalOffset;

        /// <summary>
        /// The target GameObject to follow.
        /// </summary>
        [SerializeField]
        private GameObject target;

        /// <summary>
        /// Whether or not to use smooth following.
        /// </summary>
        [SerializeField]
        private bool smoothFollow;

        /// <summary>
        /// The follow velocity when using smooth follow.
        /// </summary>
        private Vector3 followVelocity = Vector3.zero;

        /// <summary>
        /// The follow delay when using smooth follow.
        /// </summary>
        public float followTime = .5f;

        /// <summary>
        /// The target GameObject to follow.
        /// </summary>
        public GameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                this.target = value;
            }
        }

        void LateUpdate()
        {
            if (target != null)
            {
                Vector3 destination = new Vector3(target.transform.position.x, target.transform.position.y + verticalOffset, transform.position.z);
                transform.position = smoothFollow ? Vector3.SmoothDamp(transform.position, destination, ref followVelocity, followTime) : destination;

            }
        }
    }
}
