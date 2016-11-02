using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowBlocker.Interop{
	public static class WindowGetter {
		private static readonly List<WindowInfo> StaticWindows = new List<WindowInfo>(50);

		private static bool EnumWindowsProc(IntPtr hWnd, int lParam) {
			var strTitle = GetWindowText(hWnd);
			var strModule = GetWindowModuleFileName(hWnd);

			if (strTitle != "" & Dll.IsWindowVisible(hWnd)) {
				var newWindowInfo = new WindowInfo{
					WindowPointer = hWnd,
					Title = strTitle,
					Module = strModule,
					Valid = true
				};
				StaticWindows.Add(newWindowInfo);
			}

			return true;
		}

		private static readonly StringBuilder StrBuild = new StringBuilder(Dll.Maxtitle);

		private static string GetWindowText(IntPtr hWnd){
			StrBuild.Length = 0;
			var strbTitle = StrBuild;
			var nLength = Dll.GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
			strbTitle.Length = nLength;
			return strbTitle.ToString();
		}

		private static string GetWindowModuleFileName(IntPtr hWnd) {
			StrBuild.Length = 0;
			var strbTitle = StrBuild;

			int processId;
			Dll.GetWindowThreadProcessId(hWnd, out processId);

			var processPtr = Dll.OpenProcess(Dll.ProcessAccessFlags.QueryInformation | Dll.ProcessAccessFlags.VirtualMemoryRead, true, processId);

			Dll.GetModuleFileNameEx(processPtr, IntPtr.Zero, strbTitle, Dll.Maxtitle);

			return strbTitle.ToString();
		}

		public static List<WindowInfo> GetDesktopWindows() {
			lock (((System.Collections.ICollection)StaticWindows).SyncRoot) {
				StaticWindows.Clear();
				var bSuccessful = Dll.EnumDesktopWindows(IntPtr.Zero, EnumWindowsProc, IntPtr.Zero); //for current desktop
				if (bSuccessful)
					return StaticWindows.ToList();
			}

			// Get the last Win32 error code
			var nErrorCode = Marshal.GetLastWin32Error();
			var strErrMsg = string.Format("EnumDesktopWindows failed with code {0}.", nErrorCode);
			throw new Exception(strErrMsg);
		}

	}
}