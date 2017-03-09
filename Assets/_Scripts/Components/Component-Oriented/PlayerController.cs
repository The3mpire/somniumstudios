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

        void Start()
        {
            cm = GetComponent<CharacterMovement>();
            inter = GetComponent<Interactor>();
            animator = GetComponent<Animator>();
            animController = GetComponent<AnimationController2D>();
        }

        void Update()
        {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            savedDirection = input == Vector3.zero ? savedDirection : input;

            if (input.Equals(new Vector3(0f, 0f, 0f)))
                animator.SetBool("isWalking", false);
            else
                animator.SetBool("isWalking", true);

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

        void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.name.Equals("SceneChanger"))
            {
                GameManager.ChangeScene(2);
            }
        }
    }
}
