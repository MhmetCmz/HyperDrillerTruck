using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntenceArea : MonoBehaviour
{
    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Driller"))
        {
            FindObjectOfType<DrillerMovement>().speedLimit = 3f; 
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Driller"))
        { 
            FindObjectOfType<DrillerMovement>().speedLimit = 5f;
        }
    }
}
