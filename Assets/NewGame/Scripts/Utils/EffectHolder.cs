using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHolder : MonoBehaviour {
	public GameObject[] parts;

	public void playEffect(string name){
		for (int i = 0; i < parts.Length; i++) {
			if (parts[i].name == name) {
				Debug.Log ("Playing here");
				GameObject obj = Instantiate (parts [i], transform.position, Quaternion.identity);
				EffectConfig conf = obj.GetComponent<EffectConfig> ();
				conf.Play ();
			}
		}
	}
}
