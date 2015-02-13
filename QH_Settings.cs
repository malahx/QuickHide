/* 
QuickIVA
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

	public class QSettings : MonoBehaviour {

		public readonly static QSettings Instance = new QSettings();

		internal string File_settings = KSPUtil.ApplicationRootPath + "GameData/" + Quick.MOD + "/Config.txt";

		[Persistent]
		public bool isHide = false;
		[Persistent]
		public bool MouseHide = true;
		[Persistent]
		public bool BlizzyToolBar = true;
		[Persistent]
		public int TimeToKeep = 1;

		public void Save() {
			ConfigNode _temp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
			_temp.Save(File_settings);
		}
		public void Load() {
			if (File.Exists (File_settings)) {
				ConfigNode _temp = ConfigNode.Load (File_settings);
				ConfigNode.LoadObjectFromConfig (this, _temp);
				Quick.Log ("Load");
			} else {
				Save ();
			}
		}
	}
}