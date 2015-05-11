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
	public class QMods {
		public QMods (ApplicationLauncherButton button) {
			appLauncherButton = button;
			AppRef = GetAppRef (appLauncherButton);
			ModName = GetModName (appLauncherButton);
			SaveCurrentAppScenes ();
			if (!QSettings.Instance.ModHasFirstConfig.Contains (ModName)) {
				CanBePin = true;
				CanBeHide = true;
				CanSetFalse = true;
				QSettings.Instance.ModHasFirstConfig.Add (ModName);
				QuickHide.Log ("Config set to default for the mod: " + AppRef, true);
			}
			if (!isHidden && CanBeHide) {
				isHidden = QSettings.Instance.isHidden;
			}
		}
		private ApplicationLauncherButton appLauncherButton;
		internal ApplicationLauncher.AppScenes AppScenesSaved = ApplicationLauncher.AppScenes.NEVER;
		internal string ModName {
			get;
			private set;
		}
		internal string AppRef {
			get;
			private set;
		}
		internal bool isActive {
			get {
				return appLauncherButton != null && QStockToolbar.isAvailable;
			}
		}
		internal bool isTrue {
			get {
				if (!isActive) {
					return false;
				}
				return appLauncherButton.State == RUIToggleButton.ButtonState.TRUE;
			}
		}
		internal bool isFalse {
			get {
				if (!isActive) {
					return false;
				}
				return appLauncherButton.State == RUIToggleButton.ButtonState.FALSE;
			}
		}
		internal bool CanBePin {
			get {
				return QSettings.Instance.CanPin.Contains (ModName);
			}
			set {
				if (value) {
					if (!CanBePin) {
						QSettings.Instance.CanPin.Add (ModName);
					}
				} else {
					QSettings.Instance.CanPin.Remove (ModName);
				}
			}
		}
		internal bool CanBeHide {
			get {
				return QSettings.Instance.CanHide.Contains (ModName);
			}
			set {
				if (value) {
					if (!CanBeHide) {
						QSettings.Instance.CanHide.Add (ModName);
						if (QSettings.Instance.isHidden) {
							StoreAppScenes ();
						}
					}
				} else {
					QSettings.Instance.CanHide.Remove (ModName);
					if (QSettings.Instance.isHidden) {
						RestoreAppScenes ();
					}
				}
			}
		}
		internal bool CanSetFalse {
			get {
				return QSettings.Instance.CanSetFalse.Contains (ModName);
			}
			set {
				if (value) {
					if (!CanSetFalse) {
						QSettings.Instance.CanSetFalse.Add (ModName);
						if (QSettings.Instance.isHidden) {
							SetFalse ();
						}
					}
				} else {
					QSettings.Instance.CanSetFalse.Remove (ModName);
				}
			}
		}
		internal bool isHidden {
			get {
				if (!isActive) {
					return false;
				}
				return appLauncherButton.VisibleInScenes == ApplicationLauncher.AppScenes.NEVER;
			}
			set {
				if (CanBeHide) {
					if (value) {
						StoreAppScenes ();
					} else {
						RestoreAppScenes ();
					}
				}
			}
		}
		internal bool isSaved {
			get {
				return AppScenesSaved != ApplicationLauncher.AppScenes.NEVER;
			}
		}
		internal bool isStored {
			get {
				return AppScenesSaved == VisibleInScenes && isSaved;
			}
		}
		internal ApplicationLauncher.AppScenes VisibleInScenes {
			get {
				if (!isActive) {
					return ApplicationLauncher.AppScenes.NEVER;
				}
				return appLauncherButton.VisibleInScenes;
			}
		}
		internal bool isThisApp(ApplicationLauncherButton Button) {
			if (!isActive) {
				return false;
			}
			return appLauncherButton == Button;
		}
		internal static string GetModName(ApplicationLauncherButton Button) {
			if (Button == null) {
				return "None";
			}
			return Button.toggleButton.onTrue.Method.Module.Assembly.GetName ().Name;
		}
		internal static string GetAppRef(ApplicationLauncherButton Button) {
			if (Button == null) {
				return "None";
			}
			return string.Format("{0} ({1}.{2})", GetModName(Button), Button.toggleButton.onTrue.Method.DeclaringType.FullName, Button.toggleButton.onTrue.Method.Name);
		}
		internal void SaveCurrentAppScenes() {
			if (!isActive || isHidden) {
				return;
			}
			AppScenesSaved = appLauncherButton.VisibleInScenes;
		}
		internal void SetFalse() {
			if (!isActive || !isTrue) {
				return;
			}
			appLauncherButton.SetFalse ();
			QuickHide.Log ("SetFalse the AppLauncher: " + AppRef, true);
		}
		private void StoreAppScenes() {
			if (!isActive || !CanBeHide || isHidden) {
				return;
			}
			SaveCurrentAppScenes ();
			appLauncherButton.VisibleInScenes = ApplicationLauncher.AppScenes.NEVER;
			QuickHide.Log ("Store the AppLauncher: " + AppRef, true);
		}
		private void RestoreAppScenes() {
			if (!isActive || isStored) {
				return;
			}
			appLauncherButton.VisibleInScenes = AppScenesSaved;
			QuickHide.Log ("Restore the AppLauncher: " + AppRef, true);
		}
		internal void Refresh(ApplicationLauncherButton Button) {
			if (Button == null) {
				return;
			}
			appLauncherButton = Button;
			QuickHide.Log ("Refresh the AppLauncher: " + AppRef, true);
		}
	}
}