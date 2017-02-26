using UnityEngine;

namespace Somnium
{
    [RequireComponent(typeof(CharacterMovement), typeof(Interactor))]
    public sealed class PlayerController : MonoBehaviour
    {
        private Vector3 savedDirection;

        private CharacterMovement cm;

        private Interactor inter;

        void Start()
        {
            cm = GetComponent<CharacterMovement>();
            inter = GetComponent<Interactor>();
        }

        void Update()
        {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            savedDirection = input == Vector3.zero ? savedDirection : input;

            cm.Move(input);

            if (Input.GetButtonDown("Interact"))
            {
                inter.Interact(savedDirection);
            }
        }
    }
}
