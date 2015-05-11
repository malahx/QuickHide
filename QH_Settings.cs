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
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QuickHide {
	public class QSettings : MonoBehaviour {

		public readonly static QSettings Instance = new QSettings();

		internal static string FileConfig = KSPUtil.ApplicationRootPath + "GameData/" + QuickHide.MOD + "/Config.txt";

		[Persistent] public bool isHidden = false;
		[Persistent] public bool MouseHide = true;
		[Persistent] public int TimeToKeep = 2;
		[Persistent] public bool StockToolBar = true;
		[Persistent] public bool BlizzyToolBar = true;
		[Persistent] public bool StockToolBar_ModApp = true;
		[Persistent] public List<string> CanPin = new List<string>(); 
		[Persistent] public List<string> CanHide = new List<string>();
		[Persistent] public List<string> CanSetFalse = new List<string>();
		[Persistent] public List<string> ModHasFirstConfig = new List<string>();

		public void Save() {
			ConfigNode _temp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
			_temp.Save(FileConfig);
			QuickHide.Log ("Settings Saved");
		}
		public void Load() {
			if (File.Exists (FileConfig)) {
				try {
					ConfigNode _temp = ConfigNode.Load (FileConfig);
					ConfigNode.LoadObjectFromConfig (this, _temp);
				} catch {
					Save ();
				}
				QuickHide.Log ("Settings Loaded");
			} else {
				Save ();
			}
		}
	}
}