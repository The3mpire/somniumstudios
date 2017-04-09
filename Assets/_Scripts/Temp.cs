using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Somnium
{
    public class Temp : MonoBehaviour
    {
        private void Start()
        {
            DialogManager.Instance.StartDialog("DialogFiles/NewTestFormat");
        }
    }
}
