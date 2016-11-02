using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowBlocker.Interop {
	public static class Dll {

		public const int Maxtitle = 255;

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetWindowModuleFileName(IntPtr hwnd, StringBuilder lpszFileName, int cchFileNameMax);


		public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

		[DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);


		[DllImport("user32.dll", EntryPoint = "GetWindowText", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);


		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);


		[DllImport("psapi.dll")]
		public static extern int GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);


		[DllImport("user32.dll", SetLastError=true)]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		[Flags]
		public enum ProcessAccessFlags : uint {
			All = 0x001F0FFF,
			Terminate = 0x00000001,
			CreateThread = 0x00000002,
			VirtualMemoryOperation = 0x00000008,
			VirtualMemoryRead = 0x00000010,
			VirtualMemoryWrite = 0x00000020,
			DuplicateHandle = 0x00000040,
			CreateProcess = 0x000000080,
			SetQuota = 0x00000100,
			SetInformation = 0x00000200,
			QueryInformation = 0x00000400,
			QueryLimitedInformation = 0x00001000,
			Synchronize = 0x00100000
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(
			ProcessAccessFlags processAccess,
			bool bInheritHandle,
			int processId
		);

		public static IntPtr OpenProcess(System.Diagnostics.Process proc, ProcessAccessFlags flags){
			return OpenProcess(flags, false, proc.Id);
		}

		[DllImport("user32.dll", EntryPoint="GetWindowLong")]
		public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

		public const int WmSyscommand = 0x0112;
		public const int ScClose = 0xF060;

	}
}
