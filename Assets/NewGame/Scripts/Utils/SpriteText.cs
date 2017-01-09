using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteText : MonoBehaviour {

	Transform parent;
	Renderer parentRenderer;
	BattleMeta parentMeta;
	Renderer renderer;
	TextMesh text;
	List<TextMesh> outlines;
	//TextMesh outline;

	void Start(){
		parent = transform.parent;
		parentRenderer = parent.GetComponent<Renderer>();
		parentMeta = parent.GetComponent<BattleMeta>();
		renderer = GetComponent<Renderer>();
		text = GetComponent<TextMesh>();
		renderer.sortingLayerID = parentRenderer.sortingLayerID;
		renderer.sortingOrder = parentRenderer.sortingOrder;

		outlines = new List<TextMesh> ();

		float ot = .05f;

		createoutline (new Vector3(ot,ot,0));
		createoutline (new Vector3(ot,-ot,0));
		createoutline (new Vector3(-ot,ot,0));
		createoutline (new Vector3(-ot,-ot,0));
	}

	private void createoutline(Vector3 offset){
		GameObject goutline = new GameObject("outline", typeof(TextMesh));
		goutline.transform.parent = transform.parent;
		goutline.GetComponent<Renderer>().sortingLayerID = parentRenderer.sortingLayerID;
		goutline.GetComponent<Renderer>().sortingOrder = parentRenderer.sortingOrder - 1;

		TextMesh outline = goutline.GetComponent<TextMesh>();
		outline.font = text.font;
		outline.fontStyle = FontStyle.Bold;
		outline.fontSize = text.fontSize;
		outline.characterSize = text.characterSize;
		Vector3 origin = text.transform.position;
		origin.x += offset.x;
		origin.y += offset.y;
		outline.transform.position = origin;
		outline.alignment = text.alignment;
		outline.anchor = text.anchor;
		outline.transform.localScale = text.transform.localScale;
		outline.transform.localScale += new Vector3 (.02f,.02f,0);
		outline.color = Color.black;

		outlines.Add (outline);
	}

	void refresh(){
		text.text = parentMeta.getLives().ToString();
		foreach (TextMesh outline in outlines) {
			
			outline.text = text.text;
		}
	}

	void Update () {
		refresh ();
	}

	/*void LateUpdate(){

	}*/
}