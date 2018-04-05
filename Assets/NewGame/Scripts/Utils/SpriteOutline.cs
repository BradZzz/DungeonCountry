using UnityEngine;

[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour {
	public Color hitcolor = Color.clear;
	public Color fatiguecolor = Color.clear;

	private Color color;
	private SpriteRenderer spriteRenderer;

	void Awake()
	{
		color = hitcolor;
		//color = Color.red;
	}

	public void setColor(bool isHit) {
		if (isHit) {
			color = hitcolor;
		} else {
			color = fatiguecolor;
		}
	}

	public void setColor(Color banner) {
		color = banner;
	}

	public Color getColor() {
		return color;
	}

	void OnEnable() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		UpdateOutline(true);
	}

	void OnDisable() {
		UpdateOutline(false);
	}

	void Update() {
		UpdateOutline(true);
	}

	void UpdateOutline(bool outline) {
		MaterialPropertyBlock mpb = new MaterialPropertyBlock();
		spriteRenderer.GetPropertyBlock(mpb);
		mpb.SetFloat("_Outline", outline ? 1f : 0);
		mpb.SetColor("_OutlineColor", color);
		spriteRenderer.SetPropertyBlock(mpb);
	}
}