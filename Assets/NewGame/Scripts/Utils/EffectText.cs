using UnityEngine;
using System.Collections;

public class EffectText : MonoBehaviour {

	BattleMeta parentMeta;
	TextMesh text;
	Transform parent;
	Vector3 basePos;

	private IEnumerator moveup = null;

	void Start(){
		parent = transform.parent;
		parentMeta = parent.GetComponent<BattleMeta>();
		text = GetComponent<TextMesh>();

		var parentRenderer = parent.GetComponent<Renderer>();
		var renderer = GetComponent<Renderer>();
		renderer.sortingLayerName = "Effect";
		renderer.sortingOrder = parentRenderer.sortingOrder;
	}

	void refresh(){
		if (parentMeta.getEffect() != text.text) {
			if (moveup != null) {
				StopCoroutine(moveup);
			}
			basePos = parent.transform.position;
			basePos.x = transform.position.x;
			basePos.y += 1.5f;
			transform.position = basePos;
			var spriteTransform = parent.transform;
			var pos = spriteTransform.position;
			text.text = parentMeta.getEffect();
			moveup = jagged_move (transform, .05f);
			StartCoroutine(moveup);
		}
	}

	IEnumerator jagged_move(Transform origin, float speed){
		Vector3 position = origin.position;
		for (int i = 0; i < 100; i++) {
			yield return new WaitForSeconds(.05f);
			position.y += speed;
			origin.position = position;
		}
	}

	void Update () {
		refresh ();
	}
}