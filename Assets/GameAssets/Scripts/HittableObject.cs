using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableObject : MonoBehaviour
{
    private DrillerCtrl drillerCtrl;
    private DestructibleObject truckDestroyer;
    private DestructibleObject thisDestroyer;
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Driller"))
        { 
            drillerCtrl = col.GetComponentInParent<DrillerCtrl>();
            truckDestroyer = col.GetComponentInParent<DestructibleObject>();
            thisDestroyer = GetComponent<DestructibleObject>();
            if (thisDestroyer!=null) thisDestroyer.DestroyObject();
            truckDestroyer.DestroyObject();
            drillerCtrl.GameOver();
        }
    }
}
