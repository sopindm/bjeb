using System.Collections.Generic;

namespace bjeb.gui
{
	public enum Skin {
		FlagBrowser,
		Window1,
		Window2,
		Window3,
		Window4,
		Window5,
		Window6,
		Window7,
		OrbitMap,
		PlaqueDialog,
		Default
	}

	public enum Style {
		Button,
		Default,
		HorizontalSlider,
		HorizontalSliderThumb,
		Label,
		Textbox,
		TextArea,
		Toggle,
		VerticalSlider,
		VerticalSliderThumb,
		Window
	}

	public class AssetBase
	{
		private static string skinName(Skin skin)
		{
			switch(skin)
			{
			case Skin.FlagBrowser:
				return "FlagBrowserSkin";
			case Skin.Window1:
				return "KSP window 1";
			case Skin.Window2:
				return "KSP window 2";
			case Skin.Window3:
				return "KSP window 3";
			case Skin.Window4:
				return "KSP window 4";
			case Skin.Window5:
				return "KSP window 5";
			case Skin.Window6:
				return "KSP window 6";
			case Skin.Window7:
				return "KSP window 7";
			case Skin.OrbitMap:
				return "OrbitMapSkin";
			case Skin.PlaqueDialog:
				return "PlaqueDialogSkin";
			case Skin.Default:
				return "Default";
			}

			return "";
		}

#if UNITY
		private static SortedDictionary<Skin, UnityEngine.GUISkin> _skins = null;

		public static UnityEngine.GUIStyle unityStyle(Style style, Skin skin)
		{
			UnityEngine.GUISkin uskin = unitySkin(skin);

			switch(style)
			{
			case Style.Button:
				return uskin.button;
			case Style.HorizontalSlider:
				return uskin.horizontalSlider;
			case Style.HorizontalSliderThumb:
				return uskin.horizontalSliderThumb;
			case Style.Label:
				return uskin.label;
			case Style.Textbox:
				return uskin.textField;
			case Style.TextArea:
				return uskin.textArea;
			case Style.Toggle:
				return uskin.toggle;
			case Style.VerticalSlider:
				return uskin.verticalSlider;
			case Style.VerticalSliderThumb:
				return uskin.verticalSliderThumb;
			case Style.Window:
				return uskin.window;
			case Style.Default:
				return null;
			}

			return null;
		}

		public static UnityEngine.GUISkin unitySkin(Skin skin)
		{
			if(_skins == null)
				_skins = new SortedDictionary<Skin, UnityEngine.GUISkin>();

			if(skin == Skin.Default)
				return UnityEngine.GUI.skin;

			if(!_skins.ContainsKey(skin))
			{
				_skins[skin] = global::AssetBase.GetGUISkin(skinName(skin));
			}

			return _skins[skin];
		}
#endif
	}
}