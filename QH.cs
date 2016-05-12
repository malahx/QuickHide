/* 
QuickHide
Copyright 2016 Malah

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

using KSP.UI.Screens;
using System;
using UnityEngine;

namespace QuickHide {

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public partial class QuickHide : MonoBehaviour {

		public static QuickHide Instance;
		[KSPField(isPersistant = true)] internal static QBlizzyToolbar BlizzyToolbar;

		private void Awake() {
			if (Instance != null || !HighLogic.LoadedSceneIsGame) {
				Destroy (this);
				return;
			}
			Instance = this;
			if (BlizzyToolbar == null) BlizzyToolbar = new QBlizzyToolbar ();
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
			GameEvents.onGameSceneLoadRequested.Add (OnGameSceneLoadRequested);
		}

		private void Start() {
			QSettings.Instance.Load ();
			QGUI.Init ();
			BlizzyToolbar.Start ();
			StartCoroutine (AfterAllAppAdded ());
			StartEach ();
		}

		private void OnDestroy() {
			if (BlizzyToolbar != null) {
				BlizzyToolbar.OnDestroy ();
			}
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
			GameEvents.onGameSceneLoadRequested.Remove (OnGameSceneLoadRequested);
			StopEach ();
		}

		private void OnShowUI() {
			if (ApplicationLauncher.Instance != null) {
				Date = DateTime.Now;
				First = true;
			}
		}

		private void OnHideUI() {
			if (ApplicationLauncher.Instance != null) {
				First = false;
			}
		}

		private void OnGUI() {
			QGUI.OnGUI ();
		}

		protected void OnGameSceneLoadRequested(GameScenes gameScene) {
			First = false;
		}
	}
}