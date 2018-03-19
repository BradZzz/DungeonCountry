using UnityEngine;
using System.Collections;

public class CastleSceneMeta : MonoBehaviour {
	public GameObject cMeta;

//	private Color thisFlag = Color.clear;
//
//	public void plantFlag(Color flag){
//		thisFlag = flag;
//	}
//
//	public Color checkFlag(Color flag){
//		return flag;
//	}
//
//	private static Texture2D _staticRectTexture;
//	private static GUIStyle _staticRectStyle;
//
//	private static Texture2D _staticHealthTexture;
//	private static GUIStyle _staticHealthStyle;
//
//	private void OnGUI() {
//		if (thisFlag != Color.clear) {
//			Vector3 guiPosition = Camera.main.WorldToScreenPoint (transform.position);
//			guiPosition.y = Screen.height - guiPosition.y;
//
//			//Black Box Base
//			Rect bRect = new Rect (guiPosition.x - 18, guiPosition.y - 38, 
//				30 * (float)10 / (float)10 + 4, 16);
//
//			if (_staticRectTexture == null) {
//				_staticRectTexture = new Texture2D (1, 1);
//			}
//			if (_staticRectStyle == null) {
//				_staticRectStyle = new GUIStyle ();
//			}
//
//			_staticRectTexture.SetPixel (0, 0, Color.black);
//			_staticRectTexture.Apply ();
//
//			_staticRectStyle.normal.background = _staticRectTexture;
//
//			GUI.Box (bRect, GUIContent.none, _staticRectStyle);
//
//			//Health Overlay
//			Rect hRect = new Rect (guiPosition.x - 16, guiPosition.y - 35, 
//				30 * (float)10 / (float)10, 10);
//
//			if (_staticHealthTexture == null) {
//				_staticHealthTexture = new Texture2D (1, 1);
//			}
//			if (_staticHealthStyle == null) {
//				_staticHealthStyle = new GUIStyle ();
//			}
//
//			_staticHealthTexture.SetPixel (0, 0, thisFlag);
//			_staticHealthTexture.Apply ();
//
//			_staticHealthStyle.normal.background = _staticHealthTexture;
//
//			GUI.Box (hRect, GUIContent.none, _staticHealthStyle);
//		}
//	}
}
