using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DynamicSorting : MonoBehaviour
{
	void LateUpdate()
	{
		var renderer = GetComponent<Renderer>();
		renderer.sortingOrder = -(int)(transform.position.y * 1000.0f);
	}
}