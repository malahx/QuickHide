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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickHide {
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class QStockToolbar : MonoBehaviour {

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
		
		private ApplicationLauncher.AppScenes AppScenes = ApplicationLauncher.AppScenes.ALWAYS;
		public static string TexturePathHide = QuickHide.MOD + "/Textures/StockToolBar_Hide";
		public static string TexturePathShow = QuickHide.MOD + "/Textures/StockToolBar_Show";

		public static string TexturePath {
			get {
				return (QSettings.Instance.isHidden ? TexturePathShow : TexturePathHide);
			}
		}

		private void OnTrue () {
			QuickHide.Instance.HideMods (true);
		}

		private void OnFalse () {
			QuickHide.Instance.HideMods (false);
		}

		private void OnHover () {
			if (QGUI.WindowSettings || !isActive || !isHovering) {
				return;
			}
			QGUI.ShowExt ();
		}

		private void OnHoverOut () {
			if (QGUI.WindowSettings || !QGUI.WindowExt) {
				return;
			}
			if (QGUI.RectExt == new Rect () || !isHovering) {
				QGUI.HideExt ();
			}
		}

		private void OnEnable () {
		}

		private void OnDisable () {
			QGUI.HideExt ();
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
				return appLauncherButton.toggleButton.IsHovering || (QGUI.RectExt.Contains (Mouse.screenPos) && QGUI.RectExt != new Rect());
			}
		}

		internal bool isTrue {
			get {
				if (!isActive || QGUI.RectExt == new Rect()) {
					return false;
				}
				return appLauncherButton.State == RUIToggleButton.ButtonState.TRUE;
			}
		}

		internal bool isFalse {
			get {
				if (!isActive || QGUI.RectExt == new Rect()) {
					return false;
				}
				return appLauncherButton.State == RUIToggleButton.ButtonState.FALSE;
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
				if (!isActive) {
					return new Rect ();
				}
				Camera _camera = UIManager.instance.uiCameras [0].camera;
				Vector3 _pos = appLauncherButton.GetAnchor ();
				Rect _rect = new Rect (0, 0, 40, 40);
				if (ApplicationLauncher.Instance.IsPositionedAtTop) {
					_rect.x = _camera.WorldToScreenPoint (_pos).x;
				} else {
					_rect.x = _camera.WorldToScreenPoint (_pos).x - 40;
					_rect.y = Screen.height - 40;
				}
				return _rect;
			}
		}

		internal static QStockToolbar Instance {
			get;
			private set;
		}

		private void Awake() {
			if (Instance != null) {
				Destroy (this);
				return;
			}
			Instance = this;
			DontDestroyOnLoad (Instance);
			GameEvents.onGUIApplicationLauncherReady.Add (AppLauncherReady);
			GameEvents.onGUIApplicationLauncherDestroyed.Add (AppLauncherDestroyed);
			GameEvents.onLevelWasLoadedGUIReady.Add (AppLauncherDestroyed);
			QuickHide.Log ("Instantiate QStockToolbar", true);
		}
			
		private void AppLauncherReady() {
			QSettings.Instance.Load ();
			if (!Enabled) {
				return;
			}
			Init ();
		}

		private void AppLauncherDestroyed(GameScenes gameScene) {
			if (CanUseIt) {
				return;
			}
			Destroy ();
		}
		
		private void AppLauncherDestroyed() {
			Destroy ();
		}

		private void OnDestroy() {
			GameEvents.onGUIApplicationLauncherReady.Remove (AppLauncherReady);
			GameEvents.onGUIApplicationLauncherDestroyed.Remove (AppLauncherDestroyed);
			GameEvents.onLevelWasLoadedGUIReady.Remove (AppLauncherDestroyed);
			QuickHide.Log ("Destroy QStockToolbar", true);
		}

		private void Init() {
			if (isActive || !isAvailable || !CanUseIt) {
				return;
			}
			if (ModApp) {
				appLauncherButton = ApplicationLauncher.Instance.AddModApplication (new RUIToggleButton.OnTrue (this.OnTrue), new RUIToggleButton.OnFalse (this.OnFalse), new RUIToggleButton.OnHover (this.OnHover), new RUIToggleButton.OnHoverOut (this.OnHoverOut), new RUIToggleButton.OnEnable (this.OnEnable), new RUIToggleButton.OnDisable (this.OnDisable), AppScenes, GetTexture);
			} else {
				appLauncherButton = ApplicationLauncher.Instance.AddApplication (new RUIToggleButton.OnTrue (this.OnTrue), new RUIToggleButton.OnFalse (this.OnFalse), new RUIToggleButton.OnHover (this.OnHover), new RUIToggleButton.OnHoverOut (this.OnHoverOut), new RUIToggleButton.OnEnable (this.OnEnable), new RUIToggleButton.OnDisable (this.OnDisable), GetTexture);
				appLauncherButton.VisibleInScenes = AppScenes;
				ApplicationLauncher.Instance.DisableMutuallyExclusive (appLauncherButton);
			}
			ApplicationLauncher.Instance.AddOnHideCallback (OnHide);
			StartCoroutine (refresh ());
			QuickHide.Log ("Add AppLauncher", true);
		}

		private void OnHide() {
			if (QGUI.WindowSettings) {
				QGUI.ToggleSettings ();
			}
			QGUI.HideExt ();
		}

		private void Destroy() {
			if (appLauncherButton == null) {
				return;
			}
			ApplicationLauncher.Instance.RemoveModApplication (appLauncherButton);
			ApplicationLauncher.Instance.RemoveApplication (appLauncherButton);
			appLauncherButton = null;
			QuickHide.Log ("Destroy AppLauncher", true);
		}

		internal void Set(bool SetTrue, bool force = false) {
			if (!isActive) {
				return;
			}
			if (SetTrue) {
				if (appLauncherButton.State == RUIToggleButton.ButtonState.FALSE) {
					appLauncherButton.SetTrue (force);
				}
			} else {
				if (appLauncherButton.State == RUIToggleButton.ButtonState.TRUE) {
					appLauncherButton.SetFalse (force);
				}
			}
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
		}

		internal void RefreshPos() {
			if (!isActive || ModApp) {
				return;
			}
			StartCoroutine (PutInLast ());
		}

		private IEnumerator PutInLast() {
			appLauncherButton.VisibleInScenes = ApplicationLauncher.AppScenes.NEVER;
			yield return new WaitForEndOfFrame ();
			appLauncherButton.VisibleInScenes = AppScenes;
			QuickHide.Log ("PutInLast AppLauncher", true);
		}
	}
}