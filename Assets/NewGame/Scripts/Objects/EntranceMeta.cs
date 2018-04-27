using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EntranceMeta : MonoBehaviour {

	public GameObject entranceInfo;
	public Sprite image;
	Color thisFlag = Color.clear;
	bool flagVisible = true;
	public GameObject glossary;

	//These are the resources belonging to the castle on top of this entrance
	private BattleGeneralMeta castleGeneral = null;
	private Dictionary<string,int> serArmyStore;
	private List<string> arm_2;
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
		serArmyStore = new Dictionary<string,int>();
	}

	public void setGeneral(BattleGeneralMeta general){
		// For all the units in the incoming generals army, create new instances
		serArmyStore.Clear();
//		List<GameObject> new_army = new List<GameObject>();
		foreach (GameObject arm in general.getArmy()) {
			GameObject unit = glossy.findUnit (arm.name.Replace("(Clone)",""));
			serArmyStore.Add (unit.name, arm.GetComponent<BattleMeta>().getLives());
		}
//		castleGeneral.setArmy(new_army);
		castleGeneral.getResources().setResources(general.getResources().getResources());
		Debug.Log ("Finished Setting Resources");
	}

	public BattleGeneralMeta getGeneral(){
		List<GameObject> this_army = new List<GameObject>();
		foreach (KeyValuePair<string,int> armUnit in serArmyStore) {
			GameObject unit = glossy.findUnit (armUnit.Key);
			GameObject instance = Instantiate (unit) as GameObject;
			instance.SetActive (false);
			BattleMeta bMet = instance.GetComponent<BattleMeta> ();
			bMet.setLives (armUnit.Value);
			this_army.Add (instance);
		}
		castleGeneral.setArmy (this_army);
		return castleGeneral;
	}

	public void addComponent(Component component){
		if (entranceInfo == null) {
			entranceInfo = new GameObject ("Entrance");
			DontDestroyOnLoad(entranceInfo);
			entranceInfo.tag = "Entrance";
		}
		Coroutines.CopyComponent (component,entranceInfo);
	}

	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;

	private static Texture2D _staticHealthTexture;
	private static GUIStyle _staticHealthStyle;

	private int yOffset = -30;

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
