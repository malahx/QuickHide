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
using System.IO;
using UnityEngine;

namespace QuickHide {

	public class Quick : MonoBehaviour {

		public readonly static string VERSION = "2.00";
		public readonly static string MOD = "QuickHide";
		private static bool isdebug = true;

		internal static void Log(string msg) {
			if (isdebug) {
				Debug.Log (MOD + "(" + VERSION + "): " + msg);
			}
		}
		internal static void Warning(string msg) {
			if (isdebug) {
				Debug.LogWarning (MOD + "(" + VERSION + "): " + msg);
			}
		}
	}

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class QuickHide : QHide {

		private string BlizzyToolBar_TexturePath = MOD + "/Textures/BlizzyToolBar";
		private IButton BlizzyToolBar_Button;
		private bool isBlizzyToolBar {
			get {
				return ToolbarManager.ToolbarAvailable && ToolbarManager.Instance != null;
			}
		}

		private void BlizzyToolBar_Init() {
			if (isBlizzyToolBar && BlizzyToolBar_Button == null) {
				BlizzyToolBar_Button = ToolbarManager.Instance.add(MOD, MOD);
				BlizzyToolBar_Button.TexturePath = BlizzyToolBar_TexturePath;
				BlizzyToolBar_Button.ToolTip = MOD;
				BlizzyToolBar_Button.OnClick += (e) => HideToggle();
			}
		}

		private void BlizzyToolBar_Destroy() {
			if (isBlizzyToolBar && BlizzyToolBar_Button != null) {
				BlizzyToolBar_Button.Destroy ();
			}
		}
			
		private void Awake() {
			QSettings.Instance.Load ();
			GameEvents.onGUIApplicationLauncherReady.Add (OnGUIApplicationLauncherReady);
			GameEvents.onShowUI.Add (OnShowUI);
			GameEvents.onHideUI.Add (OnHideUI);
			GameEvents.onGUIAdministrationFacilitySpawn.Add (OnHideUI);
			GameEvents.onGUIMissionControlSpawn.Add (OnHideUI);
			GameEvents.onGUIRnDComplexSpawn.Add (OnHideUI);
			GameEvents.onGUIAstronautComplexSpawn.Add (OnHideUI);
			GameEvents.onGUIAdministrationFacilityDespawn.Add (OnShowUI);
			GameEvents.onGUIMissionControlDespawn.Add (OnShowUI);
			GameEvents.onGUIRnDComplexDespawn.Add (OnShowUI);
			GameEvents.onGUIAstronautComplexDespawn.Add (OnShowUI);
			if (QSettings.Instance.BlizzyToolBar && HighLogic.LoadedSceneIsGame) {
				BlizzyToolBar_Init ();
			}
		}
		private void OnDestroy() {
			GameEvents.onGUIApplicationLauncherReady.Remove (OnGUIApplicationLauncherReady);
			GameEvents.onShowUI.Remove (OnShowUI);
			GameEvents.onHideUI.Remove (OnHideUI);
			GameEvents.onGUIAdministrationFacilitySpawn.Remove (OnHideUI);
			GameEvents.onGUIMissionControlSpawn.Remove (OnHideUI);
			GameEvents.onGUIRnDComplexSpawn.Remove (OnHideUI);
			GameEvents.onGUIAstronautComplexSpawn.Remove (OnHideUI);
			GameEvents.onGUIAdministrationFacilityDespawn.Remove (OnShowUI);
			GameEvents.onGUIMissionControlDespawn.Remove (OnShowUI);
			GameEvents.onGUIRnDComplexDespawn.Remove (OnShowUI);
			GameEvents.onGUIAstronautComplexDespawn.Remove (OnShowUI);
			BlizzyToolBar_Destroy ();
		}

		private void OnGUIApplicationLauncherReady() {
			if (QSettings.Instance.isHide) {
				HideMods (QSettings.Instance.isHide);
			}
			if (QSettings.Instance.MouseHide) {
				Shown = true;
			}
		}

		private void OnShowUI() {
			if (QSettings.Instance.MouseHide && ForceHide) {
				ForceHide = false;
				Hide (false);
			}
		}

		private void OnHideUI() {
			if (HighLogic.LoadedSceneIsGame) {
				if (ApplicationLauncher.Instance != null) {
					if (QSettings.Instance.MouseHide) {
						ForceHide = true;
					}
				}
			}
		}

		private void FixedUpdate() {
			if (HighLogic.LoadedSceneIsGame) {
				if (ApplicationLauncher.Instance != null) {
					HideUpdate ();
				}
			}
		}
	}
}