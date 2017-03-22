using UnityEngine;

namespace Somnium
{
    [RequireComponent(typeof(CharacterMovement), typeof(Interactor))]
    public sealed class PlayerController : MonoBehaviour
    {
        private Vector3 savedDirection;

        private CharacterMovement cm;

        private Interactor inter;

        private Animator animator;

        private AnimationController2D animController;

        private bool controlsEnabled = true;

        public bool ControlsEnabled { get { return controlsEnabled; } set { controlsEnabled = value; } }

        void Start()
        {
            cm = GetComponent<CharacterMovement>();
            inter = GetComponent<Interactor>();
            animator = GetComponent<Animator>();
            animController = GetComponent<AnimationController2D>();
        }

        void Update()
        {
            if (ControlsEnabled)
            {
                Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                savedDirection = input == Vector3.zero ? savedDirection : input;

                // If the player is standing still - set animation to idle
                if (input.Equals(new Vector3(0f, 0f, 0f)))
                    animator.SetBool("isWalking", false);
                // Otherwise set the animation to walking
                else
                    animator.SetBool("isWalking", true);

                // Set facing direction based on x input
                if (input.x < 0)
                    animController.setFacing("Left");
                else if (input.x > 0)
                    animController.setFacing("Right");

                cm.Move(input);

                if (Input.GetButtonDown("Interact"))
                {
                    inter.Interact(savedDirection);
                }
            }
        }

        /// <summary>
        /// Place the player's location to where they were
        /// </summary>
        void OnLevelWasLoaded() {
			if (StateMachine.wasInPuzzle) {
				transform.position = GameManager.GetPreviousLocation ();
				StateMachine.wasInPuzzle = false;
			}
        }

        /// <summary>
        /// Used to change between levels
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerEnter(Collider col)
        {
            //TODO pls sir dont hardcode
            if (col.gameObject.name.Equals("SceneChanger"))
            {
                GameManager.ChangeScene(2);
            }
        }
    }
}
