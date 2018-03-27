using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using UnityEngine.SceneManagement;

public class EntranceMeta : MonoBehaviour {

	public GameObject entranceInfo;
	public Sprite image;
	Color thisFlag = Color.clear;
	bool flagVisible = true;

	public void hideFlag(){
		flagVisible = false;
	}

	public void showFlag(){
		flagVisible = true;
	}

	public void plantFlag(Color flag){
		thisFlag = flag;
	}

	public Color checkFlag(){
		return thisFlag;
	}

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public void addComponent(Component component){
		if (entranceInfo == null) {
			entranceInfo = new GameObject ("Entrance");
			DontDestroyOnLoad(entranceInfo);
			entranceInfo.tag = "Entrance";
		}
		Coroutines.CopyComponent (component,entranceInfo);
//		CastleMeta cMet = entranceInfo.GetComponent<CastleMeta> ();
//		if (cMet != null) {
//			thisFlag = Color.blue;
//			Debug.Log (this.transform.position);
//			Debug.Log (this.name);
//		}
	}

	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;

	private static Texture2D _staticHealthTexture;
	private static GUIStyle _staticHealthStyle;

	private int yOffset = -30;

	private void OnGUI() {
		if (thisFlag != Color.clear) {
			if (flagVisible) {
				Vector3 guiPosition = Camera.main.WorldToScreenPoint (transform.position);
				guiPosition.y = Screen.height - guiPosition.y;

				//Black Box Base
				Rect bRect = new Rect (guiPosition.x + 2, guiPosition.y - 38 + yOffset, 14, 15);

				if (_staticRectTexture == null) {
					_staticRectTexture = new Texture2D (1, 1);
				}
				if (_staticRectStyle == null) {
					_staticRectStyle = new GUIStyle ();
				}

				_staticRectTexture.SetPixel (0, 0, Color.black);
				_staticRectTexture.Apply ();

				_staticRectStyle.normal.background = _staticRectTexture;

				GUI.Box (bRect, GUIContent.none, _staticRectStyle);

				//Health Overlay
				Rect hRect = new Rect (guiPosition.x + 4, guiPosition.y - 35 + yOffset, 10, 9);

				if (_staticHealthTexture == null) {
					_staticHealthTexture = new Texture2D (1, 1);
				}
				if (_staticHealthStyle == null) {
					_staticHealthStyle = new GUIStyle ();
				}

				_staticHealthTexture.SetPixel (0, 0, thisFlag);
				_staticHealthTexture.Apply ();

				_staticHealthStyle.normal.background = _staticHealthTexture;

				GUI.Box (hRect, GUIContent.none, _staticHealthStyle);
			}
		}
	}
}
