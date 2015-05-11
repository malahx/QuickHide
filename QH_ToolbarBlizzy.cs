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
	public class QBlizzyToolbar {
	
		internal bool Enabled {
			get {
				return QSettings.Instance.BlizzyToolBar;
			}
		}
		public static string TexturePathShow = QuickHide.MOD + "/Textures/BlizzyToolBar_Show";
		public static string TexturePathHide = QuickHide.MOD + "/Textures/BlizzyToolBar_Hide";
		public static string TexturePathConf = QuickHide.MOD + "/Textures/BlizzyToolBar";

		public static string TexturePath {
			get {
				return (QSettings.Instance.isHidden ? TexturePathShow : TexturePathHide);
			}
		}

		private void OnClick () {
			QuickHide.Instance.HideMods ();
		}

		private IButton Button;
		private IButton ButtonConf;

		internal static bool isAvailable {
			get {
				return ToolbarManager.ToolbarAvailable && ToolbarManager.Instance != null;
			}
		}

		internal bool isActive {
			get {
				return Button != null && isAvailable;
			}
		}

		internal void Start() {
			if (!HighLogic.LoadedSceneIsGame || !isAvailable || !Enabled) {
				return;
			}
			if (Button == null) {
				Button = ToolbarManager.Instance.add (QuickHide.MOD, QuickHide.MOD);
				Button.TexturePath = TexturePath;
				Button.ToolTip = (QSettings.Instance.isHidden ? QuickHide.MOD + ": Show" : QuickHide.MOD + ": Hide");
				Button.OnClick += (e) => OnClick ();
			}
			if (ButtonConf == null) {
				ButtonConf = ToolbarManager.Instance.add (QuickHide.MOD + "Conf", QuickHide.MOD + "Conf");
				ButtonConf.TexturePath = TexturePathConf;
				ButtonConf.ToolTip = QuickHide.MOD + ": Settings";
				ButtonConf.OnClick += (e) => QGUI.Settings ();
			}
		}


		internal void OnDestroy() {
			if (!isAvailable) {
				return;
			}
			if (Button != null) {
				Button.Destroy ();
				Button = null;
			}
			if (ButtonConf != null) {
				ButtonConf.Destroy ();
				ButtonConf = null;
			}
		}

		internal void Reset() {
			if (Enabled) {
				Start ();
			} else {
				OnDestroy ();
			}
		}

		internal void Refresh() {
			if (isActive) {
				Button.TexturePath = TexturePath;
				Button.ToolTip = (QSettings.Instance.isHidden ? QuickHide.MOD + ": Show" : QuickHide.MOD + ": Hide");
			}
		}
	}
}