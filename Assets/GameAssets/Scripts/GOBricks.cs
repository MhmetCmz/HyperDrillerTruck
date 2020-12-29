using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOBricks : MonoBehaviour
{
    [SerializeField]
    Rigidbody[] brickRigids;

    public float expRad, expForce;
    bool oneShot = false;
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Driller"))
        { 
            if (!oneShot)
            {
                foreach (Rigidbody rigid in brickRigids)
                {  
                    if (rigid != null)
                    {
                        rigid.isKinematic = false; 
                        rigid.AddExplosionForce(expForce, col.transform.position /*+ new Vector3(1f,0,0)*/, expRad);
                        Destroy(rigid.gameObject, 1f);
                    }
                }
                Destroy(gameObject, 1f);
                oneShot = true;
            }
        }
    }
}
