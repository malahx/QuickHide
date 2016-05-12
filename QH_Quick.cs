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

using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace QuickHide {

	public partial class QuickHide : MonoBehaviour {

		public readonly static string VERSION = Assembly.GetAssembly(typeof(QuickHide)).GetName().Version.Major + "." + Assembly.GetAssembly(typeof(QuickHide)).GetName().Version.Minor + Assembly.GetAssembly(typeof(QuickHide)).GetName().Version.Build;
		public readonly static string MOD = Assembly.GetAssembly(typeof(QuickHide)).GetName().Name;

		internal static void Log(string _string, bool debug = false) {
			if (!debug) {
				Debug.Log (MOD + "(" + VERSION + "): " + _string);
			} else {
				#if DEBUG
				Debug.Log (MOD + "(" + VERSION + "): " + _string);
				#endif
			}
		}
		internal static void Warning(string _string, bool debug = false) {
			if (!debug) {
				Debug.LogWarning (MOD + "(" + VERSION + "): " + _string);
			} else {
				#if DEBUG
				Debug.LogWarning (MOD + "(" + VERSION + "): " + _string);
				#endif
			}
		}

		private Coroutine CoroutineEach;

		internal void StartEach() {
			if (CoroutineEach == null) {
				CoroutineEach = StartCoroutine (UpdateEach ());
			}
		}

		internal void StopEach() {
			if (CoroutineEach != null) {
				StopCoroutine (UpdateEach ());
				CoroutineEach = null;
			}
		}

		internal void RestartEach() {
			if (CoroutineEach != null) {
				StopEach ();
				StartEach ();
			}
		}

		internal IEnumerator UpdateEach () {
			yield return new WaitForSeconds (1);
			Coroutine _coroutine = CoroutineEach;
			Log ("StartUpdateEach " + _coroutine.GetHashCode(), true);
			while (_coroutine == CoroutineEach) {
				while (!QStockToolbar.isAvailable || !First || (!QSettings.Instance.isHidden && !QGUI.WindowSettings)) {
					yield return 0;
				}
				Log ("UpdateEach " + _coroutine.GetHashCode(), true);
				PopulateAppLauncherButtons ();
				yield return new WaitForSeconds (1);
			}
			Log ("EndUpdateEach " + _coroutine.GetHashCode(), true);
		}
	}
}