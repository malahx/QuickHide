/* 
QuickIVA
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
	public class QToolbar : MonoBehaviour {

		private static string MOD = Quick.MOD;
		public static bool StockToolBar = QSettings.Instance.StockToolBar;
		public static bool BlizzyToolBar = QSettings.Instance.BlizzyToolBar;
		public static ApplicationLauncher.AppScenes StockToolBar_AppScenes = ApplicationLauncher.AppScenes.SPACECENTER;
		public static GameScenes[] BlizzyToolBar_AppScenes = {
			GameScenes.SPACECENTER
		};
		public static string StockToolBar_TexturePath = Quick.MOD + "/Textures/StockToolBar";
		public static string BlizzyToolBar_TexturePath = Quick.MOD + "/Textures/BlizzyToolBar";

		private static void OnClick() { QGUI.Settings (); }




		private static Texture2D StockToolBar_Texture;
		private static ApplicationLauncherButton StockToolBar_Button;
		private static IButton BlizzyToolBar_Button;

		public static bool isBlizzyToolBar {
			get {
				return ToolbarManager.ToolbarAvailable && ToolbarManager.Instance != null;
			}
		}

		private void Awake() {
			QSettings.Instance.Load ();
			QGUI.Awake ();
			StockToolBar_Texture = GameDatabase.Instance.GetTexture (StockToolBar_TexturePath, false);
			GameEvents.onGUIApplicationLauncherReady.Add (OnGUIApplicationLauncherReady);
			if (BlizzyToolBar && HighLogic.LoadedSceneIsGame) {
				BlizzyToolBar_Init ();
			}
		}

		private void OnDestroy() {
			GameEvents.onGUIApplicationLauncherReady.Remove (OnGUIApplicationLauncherReady);
			StockToolBar_Destroy ();
			BlizzyToolBar_Destroy ();
		}

		private void OnGUIApplicationLauncherReady() {
			if (StockToolBar) {
				StockToolBar_Init ();
			}
		}

		private static void BlizzyToolBar_Init() {
			if (isBlizzyToolBar && BlizzyToolBar_Button == null) {
				BlizzyToolBar_Button = ToolbarManager.Instance.add(MOD, MOD);
				BlizzyToolBar_Button.TexturePath = BlizzyToolBar_TexturePath;
				BlizzyToolBar_Button.ToolTip = MOD;
				BlizzyToolBar_Button.OnClick += (e) => OnClick();
				BlizzyToolBar_Button.Visibility = new GameScenesVisibility(BlizzyToolBar_AppScenes);
			}
		}

		private static void BlizzyToolBar_Destroy() {
			if (isBlizzyToolBar && BlizzyToolBar_Button != null) {
				BlizzyToolBar_Button.Destroy ();
			}
		}

		private static void StockToolBar_Init() {
			if (ApplicationLauncher.Ready && StockToolBar_Button == null) {
				StockToolBar_Button = ApplicationLauncher.Instance.AddModApplication (OnClick, OnClick, null, null, null, null, StockToolBar_AppScenes, StockToolBar_Texture);
			}
		}

		private static void StockToolBar_Destroy() {
			if (StockToolBar_Button != null) {
				ApplicationLauncher.Instance.RemoveModApplication (StockToolBar_Button);
				StockToolBar_Button = null;
			}
		}

		public static void Reset() {
			if (!QGUI.WindowSettings) {
				if (StockToolBar_Button != null) {
					if (StockToolBar_Button.State == RUIToggleButton.ButtonState.TRUE) {
						StockToolBar_Button.SetFalse (false);
					}
				}
				if (QSettings.Instance.StockToolBar) {
					if (StockToolBar_Button == null) {
						StockToolBar_Init ();
					}
				} else if (StockToolBar_Button != null) {
					StockToolBar_Destroy ();
				}
				if (QSettings.Instance.BlizzyToolBar) {
					if (BlizzyToolBar_Button == null) {
						BlizzyToolBar_Init ();
					}
				} else if (BlizzyToolBar_Button != null) {
					BlizzyToolBar_Destroy ();
				}
			} else {
				if (StockToolBar_Button != null) {
					if (StockToolBar_Button.State == RUIToggleButton.ButtonState.FALSE) {
						StockToolBar_Button.SetTrue (false);
					}
				}
			}
		}
		private void OnGUI() {
			QGUI.OnGUI ();
		}
	}
}