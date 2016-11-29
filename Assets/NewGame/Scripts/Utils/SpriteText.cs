﻿using UnityEngine;
using System.Collections;

public class SpriteText : MonoBehaviour {

	void refresh(){
		var parent = transform.parent;

		var parentRenderer = parent.GetComponent<Renderer>();
		var parentMeta = parent.GetComponent<BattleMeta>();
		var renderer = GetComponent<Renderer>();
		renderer.sortingLayerID = parentRenderer.sortingLayerID;
		renderer.sortingOrder = parentRenderer.sortingOrder;

		var spriteTransform = parent.transform;
		var text = GetComponent<TextMesh>();
		var pos = spriteTransform.position;
		text.text = parentMeta.getLives().ToString();
	}

	void Update () {
		refresh ();
	}
}