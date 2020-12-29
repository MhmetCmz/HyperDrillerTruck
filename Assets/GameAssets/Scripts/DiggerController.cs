using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillController : MonoBehaviour
{ 
	ParticleSystem dirtParticle;
	private void Start()
	{
		dirtParticle = GameObject.Find("DirtParticle").GetComponent<ParticleSystem>();
	}
	private void OnTriggerStay(Collider col)
	{
		if (col.CompareTag("Clay"))
		{ 
			if (dirtParticle != null)
			{
				dirtParticle.Play();
			}
		}
	}
	private void OnTriggerExit(Collider col)
	{
		if (col.CompareTag("Clay"))
		{
			if (dirtParticle != null)
			{
				dirtParticle.Stop();
			}
		}
	}
} 
