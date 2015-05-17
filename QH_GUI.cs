/* 
QuickHide
Copyright 2015 Malah

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
*/

using System;
using UnityEngine;

namespace QuickHide {

	public partial class QGUI : MonoBehaviour {

		private static string TexturePathConf = QuickHide.MOD + "/Textures/StockToolBar";
		private static Texture2D TextureConf;

		internal static bool WindowSettings = false;
		internal static bool WindowExt = false;

		internal static Rect RectSettings = new Rect (0, 0, 900, 0);
		internal static Rect RectExt = new Rect ();
		internal static GUIStyle ExtButtonStyle;
		private static GUIStyle LabelStyle;
		private static Vector2 scrollPosition = new Vector2 ();
		private static string TimeToKeep = "2";

		internal static void Init() {
			TextureConf = GameDatabase.Instance.GetTexture (TexturePathConf, false);
			TimeToKeep = QSettings.Instance.TimeToKeep.ToString ();
		}

		internal static void RefreshStyle(bool force = false) {
			if (ExtButtonStyle == null || force) {
				ExtButtonStyle = new GUIStyle (GUI.skin.button);
				ExtButtonStyle.alignment = TextAnchor.MiddleCenter;
				ExtButtonStyle.padding = new RectOffset (0, 0, 0, 0);
				ExtButtonStyle.imagePosition = ImagePosition.ImageOnly;
			}
			if (LabelStyle == null || force) {
				LabelStyle = new GUIStyle (GUI.skin.label);
				LabelStyle.stretchWidth = true;
				LabelStyle.stretchHeight = true;
				LabelStyle.alignment = TextAnchor.MiddleCenter;
				LabelStyle.fontStyle = FontStyle.Bold;
				LabelStyle.normal.textColor = Color.white;
			}
		}

		internal static void RefreshRect() {
			if (WindowSettings) {
				RectSettings.x = (Screen.width - RectSettings.width) / 2;
				RectSettings.y = (Screen.height - RectSettings.height) / 2;
			}
			if (WindowExt) {
				RectExt.width = 40;
				RectExt.height = 46;
				RectExt.x = (Screen.width - RectExt.width) / 2;
				RectExt.y = (Screen.height - RectExt.height) / 2;
				if (QStockToolbar.isAvailable && QStockToolbar.Instance.isActive) {
					Rect _Spos = QStockToolbar.Instance.Position;
					if (_Spos != new Rect ()) {
						RectExt.x = _Spos.x;
						if (ApplicationLauncher.Instance.IsPositionedAtTop) {
							RectExt.y = _Spos.y + _Spos.height -2;
						} else {
							RectExt.y = Screen.height - _Spos.height - RectExt.height +2;
						}
					}
				}
			}
		}

		private static void Lock(bool activate, ControlTypes Ctrl) {
			if (HighLogic.LoadedSceneIsEditor) {
				if (activate) {
					EditorLogic.fetch.Lock(true, true, true, "EditorLock" + QuickHide.MOD);
				} else {
					EditorLogic.fetch.Unlock ("EditorLock" + QuickHide.MOD);
				}
			}
			if (activate) {
				InputLockManager.SetControlLock (Ctrl, "Lock" + QuickHide.MOD);
			} else {
				InputLockManager.RemoveControlLock ("Lock" + QuickHide.MOD);
			}
			if (InputLockManager.GetControlLock ("Lock" + QuickHide.MOD) != ControlTypes.None) {
				InputLockManager.RemoveControlLock ("Lock" + QuickHide.MOD);
			}
			if (InputLockManager.GetControlLock ("EditorLock" + QuickHide.MOD) != ControlTypes.None) {
				InputLockManager.RemoveControlLock ("EditorLock" + QuickHide.MOD);
			}
		}

		public static void Settings() {
			ToggleSettings ();
			if (!WindowSettings) {
				QStockToolbar.Instance.Reset ();
				QuickHide.BlizzyToolbar.Reset ();
				QSettings.Instance.Save ();
			}
		}

		internal static void ToggleSettings() {
			WindowSettings = !WindowSettings;
			Lock (WindowSettings, ControlTypes.KSC_ALL | ControlTypes.TRACKINGSTATION_UI | ControlTypes.CAMERACONTROLS | ControlTypes.MAP);
		}

		internal static void HideExt() {
			WindowExt = false;
			Lock (WindowExt, ControlTypes.KSC_ALL | ControlTypes.TRACKINGSTATION_UI | ControlTypes.CAMERACONTROLS | ControlTypes.MAP);
		}

		internal static void ShowExt() {
			WindowExt = true;
			Lock (WindowExt, ControlTypes.KSC_ALL | ControlTypes.TRACKINGSTATION_UI | ControlTypes.CAMERACONTROLS | ControlTypes.MAP);
		}

		private static void Refresh() {
			GUI.skin = HighLogic.Skin;
			RefreshStyle ();
			RefreshRect ();
		}

		internal static void OnGUI() {
			if (!WindowSettings && !WindowExt) {
				return;
			}
			Refresh ();
			if (WindowExt) {
				if (QStockToolbar.Instance.isActive) {
					if (!QStockToolbar.Instance.isHovering) {
						HideExt ();
						return;
					}
				}
				DrawExt (RectExt);
			}
			if (WindowSettings) {
				RectSettings = GUILayout.Window (1584654, RectSettings, DrawSettings, QuickHide.MOD + " " + QuickHide.VERSION, GUILayout.Width (RectSettings.width), GUILayout.ExpandHeight (true));
			}
		}

		private static void DrawSettings(int id) {
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.Box("Options",GUILayout.Height(30));
			GUILayout.EndHorizontal();
			GUILayout.Space(5);

			GUILayout.BeginHorizontal ();
			QSettings.Instance.StockToolBar = GUILayout.Toggle (QSettings.Instance.StockToolBar, "Use the Stock ToolBar", GUILayout.Width(300));
			if (QSettings.Instance.StockToolBar) {
				QSettings.Instance.StockToolBar_ModApp = !GUILayout.Toggle (!QSettings.Instance.StockToolBar_ModApp, "Put QuickHide in Stock", GUILayout.Width (300));
			} else {
				GUILayout.Space (300);
			}
			if (QBlizzyToolbar.isAvailable) {
				QSettings.Instance.BlizzyToolBar = GUILayout.Toggle (QSettings.Instance.BlizzyToolBar, "Use the Blizzy ToolBar", GUILayout.Width (300));
			}
			GUILayout.EndHorizontal ();
			GUILayout.Space (5);
			GUILayout.BeginHorizontal();
			bool _bool = GUILayout.Toggle (QSettings.Instance.MouseHide, "Enable AutoHide on mouse hovering out the Stock ToolBar");
			if (_bool != QSettings.Instance.MouseHide) {
				QSettings.Instance.MouseHide = _bool;
				RectSettings.height = 0;
			}
			GUILayout.EndHorizontal ();
			GUILayout.Space (5);
			if (QSettings.Instance.MouseHide) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Time to keep the Stock ToolBar after a mouse hovering out (in seconds):", GUILayout.Width (490));
				TimeToKeep = GUILayout.TextField (TimeToKeep, GUILayout.Width (100));
				GUILayout.EndHorizontal ();
				GUILayout.Space (5);
			}

			GUILayout.BeginHorizontal();
			GUILayout.Box("Mods",GUILayout.Height(30));
			GUILayout.EndHorizontal();
			GUILayout.Space(5);

			scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width (880), GUILayout.Height (300));
			QuickHide.Instance.DrawAppLauncherButtons ();
			GUILayout.EndScrollView ();
			GUILayout.Space (5);
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Close", GUILayout.Height(30))) {
				int _int;
				if (int.TryParse(TimeToKeep, out _int)) {
					QSettings.Instance.TimeToKeep = _int;
				} else {
					QSettings.Instance.TimeToKeep = 2;
					QuickHide.Log ("Time to keep the Stock ToolBar after a mouse hovering out is not set in seconds");
				}
				Settings ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.Space (5);
			GUILayout.EndVertical();
		}

		private static void DrawExt(Rect rect) {
			GUILayout.BeginArea (rect);
			GUILayout.BeginVertical ();
			GUILayout.Space (3);
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (new GUIContent (TextureConf), ExtButtonStyle, GUILayout.Width (40), GUILayout.Height (40))) {
				Settings ();
			}
			GUILayout.Space (3);
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
		}
	}
}