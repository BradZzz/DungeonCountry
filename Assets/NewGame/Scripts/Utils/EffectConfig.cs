using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectConfig : MonoBehaviour {
	
	void Start () {
		var renderer = GetComponent<Renderer>();
		renderer.sortingLayerName = "Effect";
	}

	public void Play() {
		//ParticleSystem ps = GetComponent<ParticleSystem> ();
		//ps.Play();
		//Destroy (gameObject,ps.duration);
		StartCoroutine (showEffects (GetComponent<ParticleSystem>()));
	}

	IEnumerator showEffects(ParticleSystem ps){
		ps.Play();
		yield return new WaitForSeconds(ps.duration - .5f);
		ps.Stop();
	}
}
