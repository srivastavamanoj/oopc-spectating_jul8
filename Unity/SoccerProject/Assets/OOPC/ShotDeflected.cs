using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotDeflected : MonoBehaviour
{




    private void OnTriggerEnter(Collider other)
    {
        // Box triggers are used to know if goalkeeper need to throw to catch the ball
        if (other.CompareTag("Ball"))
        {
            Debug.Log("333333333333333333333333333333333333333333333333");
        }
    }
}
