/* 
QuickHide
Copyright 2015 Malah

This
program is free software: you can redistribute it and/or modify
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
	public class QHide : Quick {

		internal Rect PosTop = new Rect (Screen.width - 40, 0, 40, 40);
		internal Rect PosDown = new Rect (Screen.width - 40, Screen.height - 40, 40, 40);
		internal Rect PosTopAppLaunch = new Rect (Screen.width / 2, 0, Screen.width / 2, 40);
		internal Rect PosDownAppLaunch = new Rect (Screen.width / 2, Screen.height - 40, Screen.width / 2, 40);

		internal bool Shown = false;
		internal bool ForceHide = false;

		internal DateTime Date = DateTime.Now;

		[KSPField(isPersistant = true)]
		internal ConfigNode VisibleInScenes = new ConfigNode ();

		internal void HideToggle() {
			QSettings.Instance.isHide = !QSettings.Instance.isHide;
			QSettings.Instance.Save ();
			HideMods (QSettings.Instance.isHide);
		}

		internal void Hide(bool Hide) {
			if (Hide) {
				ApplicationLauncher.Instance.Hide ();
				Shown = false;
				Warning ("Hide Applauncher");
			} else {
				ApplicationLauncher.Instance.Show ();
				Shown = true;
				Warning ("Show Applauncher");
			}
		}

		internal void HideMods(bool Hide) {
			if (ApplicationLauncher.Instance != null) {
				ApplicationLauncherButton[] AppLauncherButtons = (ApplicationLauncherButton[])Resources.FindObjectsOfTypeAll (typeof(ApplicationLauncherButton));
				foreach (ApplicationLauncherButton AppLauncherButton in AppLauncherButtons) {
					bool _hidden;
					if (ApplicationLauncher.Instance.Contains (AppLauncherButton, out _hidden)) {
						if (Hide) {
							VisibleInScenes.AddValue (AppLauncherButton.GetInstanceID ().ToString (), AppLauncherButton.VisibleInScenes.ToString ().Replace (",", string.Empty));
							AppLauncherButton.VisibleInScenes = ApplicationLauncher.AppScenes.NEVER;
							Log ("HideMods: " + AppLauncherButton.name + "(" + AppLauncherButton.GetInstanceID () + ")");
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
							Log ("UnHideMods: " + AppLauncherButton.name + "(" + AppLauncherButton.GetInstanceID () + "): " + AppLauncherButton.VisibleInScenes.ToString ());
						}
					}
				}
			}
		}

		internal void HideUpdate() {
			if (QSettings.Instance.MouseHide) {
				if (!ApplicationLauncher.Ready && !ForceHide) {
					if ((PosTop.Contains (Mouse.screenPos) && ApplicationLauncher.Instance.IsPositionedAtTop) || (PosDown.Contains (Mouse.screenPos) && !ApplicationLauncher.Instance.IsPositionedAtTop)) {
						Hide (false);
					}
				}
			}
			if ((Shown && QSettings.Instance.MouseHide) || (QSettings.Instance.isHide && !QSettings.Instance.MouseHide)) {
				bool _NeedToBeHide = true;
				if (QSettings.Instance.MouseHide) {
					if ((PosTopAppLaunch.Contains (Mouse.screenPos) && ApplicationLauncher.Instance.IsPositionedAtTop) || (PosDownAppLaunch.Contains (Mouse.screenPos) && !ApplicationLauncher.Instance.IsPositionedAtTop)) {
						_NeedToBeHide = false;
					}
				}
				ApplicationLauncherButton[] AppLauncherButtons = (ApplicationLauncherButton[])Resources.FindObjectsOfTypeAll (typeof(ApplicationLauncherButton));
				foreach (ApplicationLauncherButton AppLauncherButton in AppLauncherButtons) {
					bool _hidden;
					if (ApplicationLauncher.Instance.Contains (AppLauncherButton, out _hidden)) {
						if (QSettings.Instance.isHide) {
							if (ApplicationLauncher.Instance.DetermineVisibility (AppLauncherButton)) {
								if (VisibleInScenes.HasValue (AppLauncherButton.GetInstanceID ().ToString ())) {
									VisibleInScenes.RemoveValue (AppLauncherButton.GetInstanceID ().ToString ());
								}
								VisibleInScenes.AddValue (AppLauncherButton.GetInstanceID ().ToString (), AppLauncherButton.VisibleInScenes.ToString ().Replace (",", string.Empty));
								AppLauncherButton.VisibleInScenes = ApplicationLauncher.AppScenes.NEVER;
								Log ("Update: " + AppLauncherButton.name + "(" + AppLauncherButton.GetInstanceID () + "): " + AppLauncherButton.VisibleInScenes.ToString ());
							} 
						}
					} else if (QSettings.Instance.MouseHide) {
						if (ApplicationLauncher.Ready && Shown && _NeedToBeHide) {
							_NeedToBeHide &= AppLauncherButton.State != RUIToggleButton.ButtonState.TRUE;
						}
					}
				}
				if (QSettings.Instance.MouseHide) {
					if (ContractsApp.Instance != null) {
						if (ContractsApp.Instance.anchor.activeSelf) {
							_NeedToBeHide = false;
						}
					}
					if (MessageSystem.Instance) {
						if (MessageSystem.Instance.counterText.gameObject.activeSelf) {
							_NeedToBeHide = false;
						}
					}
					if (_NeedToBeHide) {
						if ((DateTime.Now - Date).TotalSeconds > QSettings.Instance.TimeToKeep) {
							Hide (true);
						}
					} else {
						Date = DateTime.Now;
					}
				}
			}
		}
	}
}