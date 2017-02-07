using UnityEngine;

namespace Somnium
{
    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerController : MonoBehaviour
    {
        private CharacterMovement cm;

        void Start()
        {
            cm = GetComponent<CharacterMovement>();
        }

        void Update()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            cm.Move(input);
        }
    }
}
