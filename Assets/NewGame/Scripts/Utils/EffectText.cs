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
		Vector3 scale = parent.transform.localScale;
		Debug.Log ("scale: " + scale.ToString());
		int fontScale = (int) (Mathf.Max(scale.x,scale.y));
		text.fontSize = (int) (fontScale * 16 + (Mathf.Pow(fontScale,3) * 1.2));

		Debug.Log ("parent: " + parentMeta.name);
		Debug.Log ("text font: " + text.fontSize);
		Debug.Log ("text parent scale: " + parent.transform.localScale);

		var parentRenderer = parent.GetComponent<Renderer>();
		var renderer = GetComponent<Renderer>();
		renderer.sortingLayerName = "Effect";
		renderer.sortingOrder = parentRenderer.sortingOrder;
	}

	void refresh(){
		if (parentMeta.getEffect() != text.text) {
			Debug.Log ("parent: " + parentMeta.name);
			Debug.Log ("effect: " + parentMeta.getEffect());
			if (moveup != null) {
				StopCoroutine(moveup);
			}
			text.text = parentMeta.getEffect();
			basePos = parent.transform.position;
			basePos.x = transform.position.x;
			basePos.y += 1.5f;
			transform.position = basePos;
			var spriteTransform = parent.transform;
			var pos = spriteTransform.position;
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