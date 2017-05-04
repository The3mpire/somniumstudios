using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class DragonForestManager : MonoBehaviour
    {
        private NPCFollow follow;
        private GameObject player;

        // Use this for initialization
        void Start()
        {
            follow = GetComponent<NPCFollow>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if (StateMachine.instance.GetFlag("willHelpLucie") == 1)
            {
                follow.FollowObj = player;
            }
        }
    }
}
