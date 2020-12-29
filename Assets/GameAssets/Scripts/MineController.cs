using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour
{
    private UIController uiCtrl;
    private void Start()
    {
        uiCtrl = FindObjectOfType<UIController>();
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Deformer")) 
        {   
            uiCtrl.totalDia++;
            uiCtrl.updateDiaText(); 
            var caseTransform = GameObject.Find("Case").transform;
            transform.DOMove(caseTransform.position, .3f).OnComplete(() => Destroy(gameObject));
        } 
    }
}
