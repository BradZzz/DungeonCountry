using UnityEngine;
using System.Collections;

public class ResourceMeta : MonoBehaviour {
	//the min and max values of this resource
	public int min = 2000, max = 4000;
	public string name;

	private int value;

	void Start(){
		value = Random.Range (min, max); 
	}

	public string getName(){
		return name;
	}

	public int getValue(){
		return value;
	}
}
