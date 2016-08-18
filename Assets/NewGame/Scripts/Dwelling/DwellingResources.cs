using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DwellingResources : MonoBehaviour {

	//All these resources need to have a sprite renderer, otherwise all this shit falls apart...
	public List<GameObject> resources;

	//This is a static resource, so make it a singleton
	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	//Pluck the resource object by name
	public GameObject pluck(string name){
		foreach(GameObject resource in resources){
			DwellingMeta meta = resource.GetComponent( typeof(DwellingMeta) ) as DwellingMeta;
			if (meta == null && meta.name.Equals(name)) {
				return resource;
			}
		}
		return null;
	}
}
