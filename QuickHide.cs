/* 
QuickHide
Copyright 2014 Malah

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
using System.IO;
using UnityEngine;
using Toolbar;

namespace QuickHide {
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class QuickHide : MonoBehaviour {
		public static string VERSION = "1.00";
		public static string MOD = "QuickHide";
		private static bool isdebug = true;
		private static string Path_settings = KSPUtil.ApplicationRootPath + "GameData/QuickHide/PLuginData/QuickHide/Config.txt";

		[KSPField(isPersistant = true)]
		public static bool isHide;
		[KSPField(isPersistant = true)]
		private static ConfigNode VisibleInScenes = new ConfigNode ();

		private string BlizzyToolBar_TexturePath = MOD + "/Textures/BlizzyToolBar";
		private IButton BlizzyToolBar_Button;
		private bool isBlizzyToolBar {
			get {
				return (ToolbarManager.Instance != null);
			}
		}

		private void BlizzyToolBar_Init() {
			if (isBlizzyToolBar && BlizzyToolBar_Button == null) {
				BlizzyToolBar_Button = ToolbarManager.Instance.add(MOD, MOD);
				BlizzyToolBar_Button.TexturePath = BlizzyToolBar_TexturePath;
				BlizzyToolBar_Button.ToolTip = MOD;
				BlizzyToolBar_Button.OnClick += (e) => QuickHideToggle();
			}
		}

		private void BlizzyToolBar_Destroy() {
			if (isBlizzyToolBar && BlizzyToolBar_Button != null) {
				BlizzyToolBar_Button.Destroy ();
			}
		}
			
		private void Awake() {
			GameEvents.onGUIApplicationLauncherReady.Add (OnGUIApplicationLauncherReady);
			BlizzyToolBar_Init ();
		}
		private void OnDestroy() {
			GameEvents.onGUIApplicationLauncherReady.Remove (OnGUIApplicationLauncherReady);
			BlizzyToolBar_Destroy ();
		}

		private void OnGUIApplicationLauncherReady() {
			Load ();
			if (isHide) {
				QuickHideNow ();
			}
		}

		private void Update() {
			if (isHide && ApplicationLauncher.Ready) {
				ApplicationLauncherButton[] AppLauncherButtons = (ApplicationLauncherButton[])Resources.FindObjectsOfTypeAll (typeof(ApplicationLauncherButton));
				foreach (ApplicationLauncherButton AppLauncherButton in AppLauncherButtons) {
					bool _hidden;
					if (ApplicationLauncher.Instance.Contains (AppLauncherButton, out _hidden) && ApplicationLauncher.Instance.name != AppLauncherButton.name) {
						if (AppLauncherButton.VisibleInScenes != ApplicationLauncher.AppScenes.NEVER) {
							if (VisibleInScenes.HasValue ()) {
								VisibleInScenes.RemoveValue (AppLauncherButton.GetInstanceID ().ToString ());
							}
							VisibleInScenes.AddValue (AppLauncherButton.GetInstanceID ().ToString (), AppLauncherButton.VisibleInScenes.ToString ().Replace (",", string.Empty));
							AppLauncherButton.VisibleInScenes = ApplicationLauncher.AppScenes.NEVER;
							Debug ("Update: " + AppLauncherButton.name + "(" + AppLauncherButton.GetInstanceID () + "): " + AppLauncherButton.VisibleInScenes.ToString ());
						}
					}
				}
			}
		}

		private void QuickHideToggle() {
			isHide = !isHide;
			Save ();
			QuickHideNow ();
		}

		private void QuickHideNow() {
			if (ApplicationLauncher.Ready) {
				ApplicationLauncherButton[] AppLauncherButtons = (ApplicationLauncherButton[])Resources.FindObjectsOfTypeAll (typeof(ApplicationLauncherButton));
				foreach (ApplicationLauncherButton AppLauncherButton in AppLauncherButtons) {
					bool _hidden;
					if (ApplicationLauncher.Instance.Contains (AppLauncherButton, out _hidden) && ApplicationLauncher.Instance.name != AppLauncherButton.name) {
						if (isHide) {
							VisibleInScenes.AddValue (AppLauncherButton.GetInstanceID ().ToString (), AppLauncherButton.VisibleInScenes.ToString ().Replace (",", string.Empty));
							AppLauncherButton.VisibleInScenes = ApplicationLauncher.AppScenes.NEVER;
							Debug ("Hide: " + AppLauncherButton.name + "(" + AppLauncherButton.GetInstanceID () + ")");
						} else {
							ApplicationLauncher.AppScenes _Scenes = new ApplicationLauncher.AppScenes ();
							string[] _AppScenes = VisibleInScenes.GetValue (AppLauncherButton.GetInstanceID ().ToString ()).Split (' ');
							foreach (string _AppScene in _AppScenes) {
								if (_AppScene == "ALWAYS") {
									_Scenes = ApplicationLauncher.AppScenes.ALWAYS;
									break;
								}
								if (_AppScene == "NEVER") {
									_Scenes = ApplicationLauncher.AppScenes.NEVER;
									break;
								}
								if (_AppScene == "FLIGHT") {
									_Scenes |= ApplicationLauncher.AppScenes.FLIGHT;
								}
								if (_AppScene == "MAPVIEW") {
									_Scenes |= ApplicationLauncher.AppScenes.MAPVIEW;
								}
								if (_AppScene == "SPACECENTER") {
									_Scenes |= ApplicationLauncher.AppScenes.SPACECENTER;
								}
								if (_AppScene == "SPH") {
									_Scenes |= ApplicationLauncher.AppScenes.SPH;
								}
								if (_AppScene == "TRACKSTATION") {
									_Scenes |= ApplicationLauncher.AppScenes.TRACKSTATION;
								}
								if (_AppScene == "VAB") {
									_Scenes |= ApplicationLauncher.AppScenes.VAB;
								}
							}
							AppLauncherButton.VisibleInScenes = _Scenes;
							VisibleInScenes.RemoveValue (AppLauncherButton.GetInstanceID ().ToString ());
							Debug ("UnHide: " + AppLauncherButton.name + "(" + AppLauncherButton.GetInstanceID () + "): " + AppLauncherButton.VisibleInScenes.ToString ());
						}
					}
				}
			}
		}

		private static void Save() {
			if (isHide) {
				ConfigNode _temp = new ConfigNode ();
				_temp.AddValue ("isHide", isHide);
				_temp.Save (Path_settings);
			} else {
				if (File.Exists (Path_settings)) {
					File.Delete (Path_settings);
				}
			}
		}

		public static void Load() {
			if (File.Exists (Path_settings)) {
				ConfigNode _temp = ConfigNode.Load (Path_settings);
				isHide = Convert.ToBoolean (_temp.GetValue ("isHide"));
			}
		}

		private static void Debug(string String) {
			if (isdebug) {
				UnityEngine.Debug.Log (MOD + "(" + VERSION + "): " + String);
			}
		}
	}
}