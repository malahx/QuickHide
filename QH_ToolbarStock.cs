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

using KSP.UI;
using KSP.UI.Screens;
using System.Collections;
using UnityEngine;

namespace QuickHide {

	public partial class QStockToolbar {

		internal static bool Enabled {
			get {
				return QSettings.Instance.StockToolBar;
			}
		}
			
		private static bool ModApp {
			get {
				return QSettings.Instance.StockToolBar_ModApp;
			}
		}

		private static bool CanUseIt {
			get {
				return HighLogic.LoadedSceneIsGame;
			}
		}
		
		private ApplicationLauncher.AppScenes AppScenes = ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB;
		public static string TexturePathHide = MOD + "/Textures/StockToolBar_Hide";
		public static string TexturePathShow = MOD + "/Textures/StockToolBar_Show";

		public static string TexturePath {
			get {
				return (QSettings.Instance.isHidden ? TexturePathShow : TexturePathHide);
			}
		}

		private void OnTrue () {
			QHide.Instance.HideMods (true);
		}

		private void OnFalse () {
			QHide.Instance.HideMods (false);
		}

		private Texture2D GetTexture {
			get {
				return GameDatabase.Instance.GetTexture(TexturePath, false);
			}
		}

		private ApplicationLauncherButton appLauncherButton;

		internal static bool isAvailable {
			get {
				return ApplicationLauncher.Ready && ApplicationLauncher.Instance != null;
			}
		}

		internal static bool isModApp(ApplicationLauncherButton button) {
			bool _hidden;
			return ApplicationLauncher.Instance.Contains (button, out _hidden);
		}

		internal bool isActive {
			get {
				return appLauncherButton != null && isAvailable;
			}
		}

		internal bool isHovering {
			get {
				if (!isActive || !CanUseIt) {
					return false;
				}
				return appLauncherButton.IsHovering;
			}
		}

		internal bool isTrue {
			get {
				if (!isActive) {
					return false;
				}
				return appLauncherButton.toggleButton.CurrentState == UIRadioButton.State.True;
			}
		}

		internal bool isFalse {
			get {
				if (!isActive) {
					return false;
				}
				return appLauncherButton.toggleButton.CurrentState == UIRadioButton.State.False;
			}
		}

		internal bool isThisApp(ApplicationLauncherButton AppLauncherButton) {
			if (AppLauncherButton == null) {
				return false;
			}
			return appLauncherButton.GetInstanceID () == AppLauncherButton.GetInstanceID ();
		}

		internal Rect Position {
			get {
				if (appLauncherButton == null || !isAvailable) {
					return new Rect ();
				}
				Camera _camera = UIMainCamera.Camera;
				Vector3 _pos = _camera.WorldToScreenPoint (appLauncherButton.GetAnchorUL());
				return new Rect (_pos.x, Screen.height - _pos.y, 41, 41);
			}
		}

		internal static QStockToolbar Instance {
			get;
			private set;
		}

		protected override void Awake() {
			if (Instance != null) {
				Destroy (this);
				return;
			}
			Instance = this;
			DontDestroyOnLoad (Instance);
			GameEvents.onGUIApplicationLauncherReady.Add (AppLauncherReady);
			GameEvents.onGUIApplicationLauncherDestroyed.Add (AppLauncherDestroyed);
			GameEvents.onLevelWasLoadedGUIReady.Add (AppLauncherDestroyed);
			Log ("Awake", "QStockToolbar");
		}
			
		private void AppLauncherReady() {
			QSettings.Instance.Load ();
			if (!Enabled) {
				return;
			}
			Init ();
			Log ("AppLauncherReady", "QStockToolbar");
		}

		private void AppLauncherDestroyed(GameScenes gameScene) {
			if (CanUseIt) {
				return;
			}
			AppLauncherDestroyed ();
		}
		
		private void AppLauncherDestroyed() {
			Destroy ();
			Log ("AppLauncherDestroyed", "QStockToolbar");
		}

		protected override void OnDestroy() {
			GameEvents.onGUIApplicationLauncherReady.Remove (AppLauncherReady);
			GameEvents.onGUIApplicationLauncherDestroyed.Remove (AppLauncherDestroyed);
			GameEvents.onLevelWasLoadedGUIReady.Remove (AppLauncherDestroyed);
			Log ("OnDestroy", "QStockToolbar");
		}

		private void Init() {
			if (isActive || !isAvailable || !CanUseIt) {
				return;
			}
			if (ModApp) {
				appLauncherButton = ApplicationLauncher.Instance.AddModApplication (OnTrue, OnFalse, null, null, null, null, AppScenes, GetTexture);
			} else {
				appLauncherButton = ApplicationLauncher.Instance.AddApplication (OnTrue, OnFalse, null, null, null, null, GetTexture);
				appLauncherButton.VisibleInScenes = AppScenes;
				ApplicationLauncher.Instance.DisableMutuallyExclusive (appLauncherButton);
			}
			appLauncherButton.onRightClick = delegate { QHide.Instance.Settings (); };
			ApplicationLauncher.Instance.AddOnHideCallback (OnHide);
			StartCoroutine (refresh ());
			Log ("Init", "QStockToolbar");
		}

		private void OnHide() {
			if (QHide.Instance.WindowSettings) {
				QHide.Instance.ToggleSettings ();
				Log ("OnHide", "QStockToolbar");
			}
		}

		private void Destroy() {
			if (appLauncherButton == null) {
				return;
			}
			ApplicationLauncher.Instance.RemoveModApplication (appLauncherButton);
			ApplicationLauncher.Instance.RemoveApplication (appLauncherButton);
			appLauncherButton = null;
			Log ("Destroy", "QStockToolbar");
		}

		internal void Set(bool SetTrue, bool force = false) {
			if (!isActive) {
				return;
			}
			if (SetTrue) {
				if (isFalse) {
					appLauncherButton.SetTrue (force);
				}
			} else {
				if (isTrue) {
					appLauncherButton.SetFalse (force);
				}
			}
			Log ("Set " + SetTrue + " force: " + force, "QStockToolbar");
		}

		internal void Reset() {
			if (appLauncherButton != null) {
				if (!Enabled || (Enabled && (ModApp && !isModApp (appLauncherButton)) || (!ModApp && isModApp (appLauncherButton)))) {
					Destroy ();
				} else {
					Set (QSettings.Instance.isHidden);
				}
			}
			if (Enabled) {
				Init ();
			}
			Log ("Reset", "QStockToolbar");
		}

		internal IEnumerator refresh() {
			yield return new WaitForEndOfFrame();
			Refresh ();
		}

		internal void Refresh() {
			if (!isActive) {
				return;
			}
			appLauncherButton.SetTexture (GetTexture);
			Set (QSettings.Instance.isHidden);
			Log ("Refresh", "QStockToolbar");
		}

		internal void RefreshPos() {
			if (!isActive || ModApp) {
				return;
			}
			StartCoroutine (PutInLast ());
			Log ("RefreshPos", "QStockToolbar");
		}

		private IEnumerator PutInLast() {
			appLauncherButton.VisibleInScenes = ApplicationLauncher.AppScenes.NEVER;
			yield return new WaitForEndOfFrame ();
			appLauncherButton.VisibleInScenes = AppScenes;
			Log ("PutInLast", "QStockToolbar");
		}
	}
}