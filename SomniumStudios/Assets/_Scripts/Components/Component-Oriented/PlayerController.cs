using UnityEngine;

namespace Somnium
{
    [RequireComponent(typeof(CharacterMovement), typeof(Interactor))]
    public sealed class PlayerController : MonoBehaviour
    {
        private Vector2 savedDirection;

        private CharacterMovement cm;

        private Interactor inter;

        void Start()
        {
            cm = GetComponent<CharacterMovement>();
            inter = GetComponent<Interactor>();
        }

        void Update()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            savedDirection = input == Vector2.zero ? savedDirection : input;

            cm.Move(input);

            if (Input.GetButtonDown("Interact"))
            {
                inter.Interact(savedDirection);
            }
        }
    }
}
