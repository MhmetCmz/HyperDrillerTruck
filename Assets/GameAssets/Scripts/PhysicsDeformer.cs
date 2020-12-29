using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsDeformer : MonoBehaviour {

	[SerializeField]
	private bool isItForDesign;
	DigdablePlane digdablePlane;
	 
    private void OnTriggerStay(Collider col)
	{
		if (col.CompareTag("Clay"))
		{
			digdablePlane = col.GetComponent<DigdablePlane>();
			digdablePlane.AddDepression(transform.position,transform.localScale);
			if (isItForDesign)
			{
				Destroy(gameObject);
			}
		}
	} 
}
