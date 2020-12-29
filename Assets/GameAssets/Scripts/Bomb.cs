using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bomb : MonoBehaviour
{  
    [SerializeField] private ParticleSystem expParticle,laserParticle;
    [SerializeField] private float expForce, expRad;

    private GameObject digger;
    private bool oneShot;

    private DrillerCtrl drillerControl;
    private DestructibleObject destructible;
    void Start()
    { 
        oneShot = false; 
        digger = GameObject.Find("Digger");
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Driller"))
        {
            drillerControl = col.GetComponentInParent<DrillerCtrl>();
            destructible = col.GetComponentInParent<DestructibleObject>();
            destructible.DestroyObject();
            drillerControl.GameOver();
            Explode(col.transform.position + new Vector3(1, -1, 0));
        }
    }

    public void Explode(Vector3 expPoint)
    {
        if (digger!=null) Destroy(digger);  
        if (!oneShot)
        {
            expParticle.Play();
            laserParticle = GetComponentInParent<ParticleSystem>();
            if (laserParticle != null) Destroy(laserParticle);
            gameObject.GetComponent<MeshRenderer>().enabled = false; 

            Collider[] colliders = Physics.OverlapSphere(transform.position, expRad);
            foreach (Collider Piece in colliders)
            {
                if (Piece.CompareTag("Pieces"))
                {
                    Piece.isTrigger = true;
                    Rigidbody rb = Piece.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddExplosionForce(expForce, expPoint, expRad);
                        Destroy(rb.gameObject, 5f);
                    }
                }
            }

            Destroy(gameObject, 2f);
            oneShot = true;
        }
    }
}

