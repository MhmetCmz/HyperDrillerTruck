using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBombController : MonoBehaviour
{
    [SerializeField]
    private Bomb[] bombs;
    
    private DrillerCtrl drillerCtrl;
    private DestructibleObject destructibleObj;

    private void Start()
    {
        bombs = GetComponentsInChildren<Bomb>();
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Driller"))
        {
            drillerCtrl = col.GetComponentInParent<DrillerCtrl>();
            destructibleObj = col.GetComponentInParent<DestructibleObject>();

            destructibleObj.DestroyObject();
            drillerCtrl.GameOver();

            Destroy(GetComponent<ParticleSystem>());

            foreach (Bomb bomb in bombs)
            { 
                bomb.Explode(col.transform.position + new Vector3(1, -1, 0)); 
            }
            Destroy(gameObject, 2f);
        }
    }
}
