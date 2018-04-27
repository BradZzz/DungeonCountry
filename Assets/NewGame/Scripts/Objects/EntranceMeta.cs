using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using UnityEngine.SceneManagement;

public class EntranceMeta : MonoBehaviour {

	public GameObject entranceInfo;
	public Sprite image;
	Color thisFlag = Color.clear;
	bool flagVisible = true;
	public GameObject glossary;

	//These are the resources belonging to the castle on top of this entrance
	private BattleGeneralMeta castleGeneral = null;
	private Glossary glossy;

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
		castleGeneral = GetComponent<BattleGeneralMeta> ();
		glossy = glossary.GetComponent<Glossary> ();
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

	public void saveEntranceResources(){
//		BattleSerializeable bGen = DataStoreConverter.serializeGeneral (castleGeneral);
//		DataStoreConverter.putKey (JsonUtility.ToJson (bGen), getID());
		Debug.Log("Entrance Saving: " + getID ());
		CastleConverter.saveEntrance (getID (), castleGeneral);
	}

	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;

	private static Texture2D _staticHealthTexture;
	private static GUIStyle _staticHealthStyle;

	private int yOffset = -30;

	void LateUpdate () {
		if(DataStoreConverter.checkKey(getID ())){
			BattleGeneralMeta bgm = CastleConverter.getEntrance (getID (), glossy);
			if (bgm != null) {
				Debug.Log ("Setting Army For Castle: " + getID ());
				for (int i = 0; i < bgm.getArmy ().Count; i++) {
					Debug.Log (bgm.getArmy()[0].GetComponent<BattleMeta>().name);
				}
				castleGeneral.setArmy (bgm.getArmy());
				castleGeneral.setResources (bgm.getResources());
			} else {
				Awake ();
			}
		}
	}

	public string getID(){
		return GetInstanceID ().ToString () + "-castle";
	}

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
