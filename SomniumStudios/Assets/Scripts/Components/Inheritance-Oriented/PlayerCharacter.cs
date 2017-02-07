using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class PlayerCharacter : MovingCharacter
    {
        protected override void Update()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Move(input);
        }
    }
}
