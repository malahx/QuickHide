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
	public partial class QuickHide : MonoBehaviour {

		[KSPField(isPersistant = true)] internal static List<QMods> ModsToolbar = new List<QMods> ();
		private DateTime Date = DateTime.Now;
		private bool First = false;

		// For FAR compatibility
		private bool EditorhasRootPart {
			get {
				if (!HighLogic.LoadedSceneIsEditor) {
					return true;
				}
				return EditorLogic.RootPart != null;
			}
		}

		private Rect StockToolBar_Position {
			get {
				if (ApplicationLauncher.Instance == null) {
					return new Rect ();
				}
				if (!ApplicationLauncher.Ready) {
					if (ApplicationLauncher.Instance.IsPositionedAtTop) {
						return new Rect (Screen.width - 40, 0, 40, 40);
					} else {
						int _width = (HighLogic.LoadedSceneIsEditor ? 75 + 40 : 40);
						return new Rect (Screen.width - _width, Screen.height - 40, _width, 40);
					}
				} else {
					if (ApplicationLauncher.Instance.IsPositionedAtTop) {
						return new Rect (Screen.width / 2, 0, Screen.width / 2, 40);
					} else {
						return new Rect (Screen.width / 2, Screen.height - 40, Screen.width / 2, 40);
					}
				}
			}
		}

		private IEnumerator AfterAllAppAdded() {
			while (ApplicationLauncher.Instance == null) {
				yield return 0;
			}
			while (!ApplicationLauncher.Ready) {
				yield return 0;
			}
			yield return new WaitForSeconds (0.5f);
			if (QStockToolbar.Instance != null) {
				QStockToolbar.Instance.RefreshPos ();
			}
			PopulateAppLauncherButtons (true);
			yield return new WaitForSeconds (0.5f);
			First = true;
		}

		private void PopulateAppLauncherButtons(bool force = false) {
			if (!QStockToolbar.isAvailable || (!First && !force)) {
				return;
			}
			QuickHide.Warning ("Begin PopulateAppLauncherButtons", true);
			bool _cansave = false;
			ApplicationLauncherButton[] _appLauncherButtons = (ApplicationLauncherButton[])Resources.FindObjectsOfTypeAll (typeof(ApplicationLauncherButton));
			foreach (ApplicationLauncherButton _appLauncherButton in _appLauncherButtons) {
				if (!QStockToolbar.isModApp (_appLauncherButton) || QStockToolbar.Instance.isThisApp (_appLauncherButton)) {
					continue;
				}
				QuickHide.Warning ("Mods: " + QMods.GetModName(_appLauncherButton) + " " + _appLauncherButton.GetInstanceID(), true);
				QMods _QData = ModsToolbar.Find (q => q.AppRef == QMods.GetAppRef(_appLauncherButton));
				if (_QData != null) {
					if (_QData.ModName != "None" && _QData.isActive) {
						if (!_QData.isThisApp(_appLauncherButton)) {
							_QData.Refresh (_appLauncherButton);
						}
						if (_QData.isHidden != QSettings.Instance.isHidden) {
							_QData.isHidden = QSettings.Instance.isHidden;
						}
						continue;
					}
					ModsToolbar.Remove (_QData);
					Log ("Deleted an lost AppLauncherButton", true);
				}
				_QData = new QMods (_appLauncherButton);
				ModsToolbar.Add (_QData);
				_cansave = true;
				Log ("Added the AppLauncherButton of: " + _QData.ModName, true);
			}
			if (_cansave) {
				QSettings.Instance.Save ();
			}
			QuickHide.Warning ("End PopulateAppLauncherButtons", true);
			QuickHide.Warning ("AppLauncherButtons count: " + _appLauncherButtons.Length, true);
			QuickHide.Warning ("ModsToolbar count: " + ModsToolbar.Count, true);
			QuickHide.Warning ("ModHasFirstConfig count: " + QSettings.Instance.ModHasFirstConfig.Count, true);
		}

		internal void DrawAppLauncherButtons() {
			List <string> _modsName = new List<string> ();
			foreach (QMods _qMods in ModsToolbar) {
				if (_qMods == null) {
					continue;
				}
				if (_qMods.ModName == "None" || _modsName.Contains(_qMods.ModName)) {
					continue;
				}
				_modsName.Add (_qMods.ModName);
				GUILayout.BeginHorizontal();
				GUILayout.Label (string.Format("<b>{0}</b>", _qMods.ModName), GUILayout.Width(200));
				bool _CanBePin = _qMods.CanBePin;
				_CanBePin = GUILayout.Toggle (_CanBePin, "Can be pin for the AutoHide", GUILayout.Width(225));
				if (_CanBePin != _qMods.CanBePin) {
					_qMods.CanBePin = _CanBePin;
				}
				bool _CanBeHide = _qMods.CanBeHide;
				_CanBeHide = GUILayout.Toggle (_CanBeHide, "Can be hidden", GUILayout.Width(140));
				if (_CanBeHide != _qMods.CanBeHide) {
					_qMods.CanBeHide = _CanBeHide;
				}
				if (_CanBeHide) {
					bool _CanSetFalse = _qMods.CanSetFalse;
					_CanSetFalse = GUILayout.Toggle (_CanSetFalse, "Set to False the button on hide", GUILayout.Width (250));
					if (_CanSetFalse != _qMods.CanSetFalse) {
						_qMods.CanSetFalse = _CanSetFalse;
					}
				}
				GUILayout.EndHorizontal();
				#if DEBUG
				GUILayout.BeginHorizontal();
				GUILayout.Space(210);
				GUILayout.Label("VisibleInScenes: " + _qMods.VisibleInScenes);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space(210);
				GUILayout.Label("AppScenesSaved: " + _qMods.AppScenesSaved);
				GUILayout.EndHorizontal();
				#endif
			}
		}

		public void HideMods() {
			HideMods (!QSettings.Instance.isHidden);
		}

		public void HideMods(bool value) {
			if (!QSettings.Instance.isHidden && value) {
				PopulateAppLauncherButtons ();
			}
			foreach (QMods _qMods in ModsToolbar) {
				if (!_qMods.CanBeHide) {
					continue;
				}
				if (value) {
					if (_qMods.CanSetFalse) {
						_qMods.SetFalse ();
					}
				}
				_qMods.isHidden = value;
			}
			if (QSettings.Instance.isHidden != value) {
				QSettings.Instance.isHidden = value;
				QSettings.Instance.Save ();
				QStockToolbar.Instance.Refresh ();
				QuickHide.BlizzyToolbar.Refresh ();
			}
			Log((QSettings.Instance.isHidden ? "Hide mods buttons" : "Show mods buttons"));
				
		}

		private void Hide(bool Hide) {
			if (ApplicationLauncher.Instance == null) {
				return;
			}
			if (Hide) {
				ApplicationLauncher.Instance.Hide ();
				Log ("Hide the AppLauncher");
			} else {
				ApplicationLauncher.Instance.Show ();
				Log ("Show the AppLauncher");
			}
		}

		private bool isPinned {
			get {
				if (QGUI.WindowExt || QGUI.WindowSettings) {
					return true;
				}
				if (MessageSystem.Instance) {
					if (MessageSystem.Instance.counterText.gameObject.activeSelf) {
						return true;
					}
				}
				foreach (QMods _qMods in ModsToolbar) {
					if (_qMods == null) {
						continue;
					}
					if (!_qMods.CanBePin || !_qMods.isActive) {
						continue;
					}
					if (_qMods.isTrue) {
						return true;
					}
				}
				if (HighLogic.LoadedSceneIsFlight) {
					if (ResourceDisplay.Instance != null) {
						if (ResourceDisplay.Instance.appLauncherButton != null) {
							if (ResourceDisplay.Instance.appLauncherButton.State == RUIToggleButton.ButtonState.TRUE) {
								return true;
							}
							ResourceDisplayOptions _resourceDisplayOptions = (ResourceDisplayOptions)ResourceDisplay.FindObjectOfType (typeof(ResourceDisplayOptions));
							if (_resourceDisplayOptions != null) {
								if (_resourceDisplayOptions.gameObject != null) {
									if (_resourceDisplayOptions.gameObject.activeSelf) {
										return true;
									}
								}
							}
						}
					}
					if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX) {
						CurrencyWidgetsApp _currencyWidgetsApp = (CurrencyWidgetsApp)CurrencyWidgetsApp.FindObjectOfType (typeof(CurrencyWidgetsApp));
						if (_currencyWidgetsApp != null) {
							if (_currencyWidgetsApp.appLauncherButton != null) {
								if (_currencyWidgetsApp.appLauncherButton.State == RUIToggleButton.ButtonState.TRUE) {
									return true;
								}
								if (_currencyWidgetsApp.widgetSpawner != null) {
									if (_currencyWidgetsApp.widgetSpawner.gameObject != null) {
										if (_currencyWidgetsApp.widgetSpawner.gameObject.activeSelf) {
											return true;
										}
									}
								}
							}
						}
					}
				}
				if (HighLogic.LoadedSceneIsEditor) {
					if (EngineersReport.Ready && EngineersReport.Instance != null) {
						if (EngineersReport.Instance.appLauncherButton != null) {
							if (EngineersReport.Instance.appLauncherButton.State == RUIToggleButton.ButtonState.TRUE) {
								return true;
							}
							GenericAppFrame _genericAppFrame = (GenericAppFrame)EngineersReport.FindObjectOfType (typeof(GenericAppFrame));
							if (_genericAppFrame != null) {
								if (_genericAppFrame.gameObject != null) {
									if (_genericAppFrame.gameObject.activeSelf) {
										return true;
									}
								}
							}
						}
					}
				}
				if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
					if (ContractsApp.Instance != null) {
						if (ContractsApp.Instance.appLauncherButton != null) {
							if (ContractsApp.Instance.appLauncherButton.State == RUIToggleButton.ButtonState.TRUE) {
								return true;
							}
							GenericAppFrame _genericAppFrame = (GenericAppFrame)ContractsApp.FindObjectOfType (typeof(GenericAppFrame));
							if (_genericAppFrame != null) {
								if (_genericAppFrame.gameObject != null) {
									if (_genericAppFrame.gameObject.activeSelf) {
										return true;
									}
								}
							}
						}
					}
				}
				return false;
			}
		}

		private void LateUpdate() {
			if (!QSettings.Instance.MouseHide || ApplicationLauncher.Instance == null || !First) {
				return;
			}
			if (!ApplicationLauncher.Ready) {
				if (StockToolBar_Position.Contains (Mouse.screenPos) || !EditorhasRootPart) {
					Date = DateTime.Now;
					Hide (false);
					return;
				}
			} else {
				if (!StockToolBar_Position.Contains (Mouse.screenPos) && EditorhasRootPart) {
					if ((DateTime.Now - Date).TotalSeconds > QSettings.Instance.TimeToKeep) {
						if (!isPinned) {
							Hide (true);
						}
					} else {
						return;
					}
				}
				Date = DateTime.Now;
			}
		}
	}
}