using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
	public class PaletteManager
	{
		Dictionary<string, PaletteCache> pStore;

		public PaletteManager ()
		{
			pStore = new Dictionary<string, PaletteCache> ();
		}

		public void CreatePalette(string name, SpriteRenderer renderer)
		{
			pStore.Add (name, new PaletteCache (renderer, BuildPalette(renderer.sprite.texture)));
		}

		public void printPalette(String key){
			if (pStore.ContainsKey(key)) {
				pStore [key].printColors ();
			}
		}

		public bool SwitchColor(string key, Color origColor, Color switchColor){
			if (pStore.ContainsKey(key)) {
				pStore [key].setSwitch (origColor, switchColor);
				return true;
			}
			return false;
		}

		public Texture2D generateSprite(string key){
			if (pStore.ContainsKey(key)) {
				return ReplaceColorSprite(pStore [key]);
			}
			return null;
		}

		private Texture2D ReplaceColorSprite (PaletteCache pCache)
		{
			Texture2D texture = pCache.getRenderer().sprite.texture;

			var w = texture.width;
			var h = texture.height;

			var cloneTexture = new Texture2D (w, h);
			cloneTexture.wrapMode = TextureWrapMode.Clamp;
			cloneTexture.filterMode = FilterMode.Point;

			var colors = texture.GetPixels ();

			foreach(KeyValuePair<Color, Color> entry in pCache.getSwitch ())
			{
				if (entry.Key != entry.Value) {
					pCache.getOrig () [entry.Key].ForEach (pixel => colors [pixel] = entry.Value);
				}
			}

			cloneTexture.SetPixels (colors);
			cloneTexture.Apply ();

			return cloneTexture;
		}

		private Dictionary<Color, List<int>> BuildPalette(Texture2D texture){
			Dictionary<Color, List<int>> palette = new Dictionary<Color, List<int>>();
			var colors = texture.GetPixels ();
			for (int i = 0; i < colors.Length; i++){
				if(palette.ContainsKey(colors[i])){
					palette[colors[i]].Add(i);
				} else {
					palette[colors[i]] = new List<int>();
					palette[colors[i]].Add(i);
				}
			}
			return palette;
		}

		private class PaletteCache {

			private Dictionary<Color, List<int>> origPalette = new Dictionary<Color, List<int>>();
			private Dictionary<Color, Color> switchPalette = new Dictionary<Color, Color>();
			SpriteRenderer renderer;

			public PaletteCache(SpriteRenderer renderer, Dictionary<Color, List<int>> palette){
				this.renderer = renderer;
				origPalette = new Dictionary<Color, List<int>>(palette);
				switchPalette = new Dictionary<Color, Color> ();
				foreach(KeyValuePair<Color, List<int>> entry in origPalette)
				{
					switchPalette.Add(entry.Key, entry.Key);

				}
			}

			public void printColors() {
				foreach(KeyValuePair<Color, List<int>> entry in origPalette)
				{
					Debug.Log ("Color: " + ((Color)entry.Key).ToString());
				}
			}
			public SpriteRenderer getRenderer(){ return renderer; }
			public Dictionary<Color, List<int>> getOrig(){ return origPalette; }
			public Dictionary<Color, Color> getSwitch(){ return switchPalette; }

			public void setSwitch(Color originColor, Color switchColor){ 
				if(switchPalette.ContainsKey(originColor)){
					switchPalette[originColor] = switchColor;
				}
			}
			private bool CompareColor(Color checking, Color looking){
				if(Mathf.Approximately(checking.r, looking.r) &&
					Mathf.Approximately(checking.g, looking.g) &&
					Mathf.Approximately(checking.b, looking.b) && checking.a == 1f){
					Debug.Log ("Match!!!");
					return true;
				}
				return false;
			}
		}
	}
}