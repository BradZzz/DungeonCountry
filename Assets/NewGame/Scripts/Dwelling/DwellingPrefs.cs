using UnityEngine;
using System.Collections;

//This is the holder for when the dwelling screen is started
public class DwellingPrefs : MonoBehaviour {

	public static DwellingPrefs Instance;

	//Hold onto the character that stepped into the dwelling
	private static gOName characterInstance = null;

	private static dInfo dwellingInfo = null;

	void Awake ()   
	{
		if (Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy (gameObject);
		}
	}

	public static string getPlayerName(){
		if (characterInstance != null) {
			return characterInstance.name;
		}
		return "";
	}

	public static void setPlayerName(string name){
		if (characterInstance == null) {
			characterInstance = new gOName ();
		}
		characterInstance.name = name;
	}

	public static void setDwellingInfo(Sprite sprite, DwellingMeta dMeta){
		if (dwellingInfo == null) {
			dwellingInfo = new dInfo ();
		}
		dwellingInfo.sprite = sprite;
		dwellingInfo.dMeta = dMeta;
	}

	public static Sprite getDwellingRenderer(){
		if (dwellingInfo != null) {
			return dwellingInfo.sprite;
		}
		return null;
	}

	public static DwellingMeta getDwellingMeta(){
		if (dwellingInfo != null) {
			return dwellingInfo.dMeta;
		}
		return null;
	}

	private class gOName{
		public string name;
		public gOName() {
			name = "";
		}
	}

	private class dInfo{
		public Sprite sprite;
		public DwellingMeta dMeta;
		public dInfo() {
			sprite = null;
			dMeta = null;
		}
	}
}
