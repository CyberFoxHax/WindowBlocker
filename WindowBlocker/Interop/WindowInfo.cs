using System;
using System.Linq;

namespace WindowBlocker.Interop{
	public struct WindowInfo {
		public bool Valid { get; set; }
		public IntPtr WindowPointer { get; set; }
		public string Title { get; set; }
		public string Module { get; set; }

		public void CloseWindow(Views.CloseMode closeMode){
			switch (closeMode){
				case Views.CloseMode.Close:
					Dll.SendMessage(WindowPointer, Dll.WmSyscommand, Dll.ScClose, 0);
					break;
				case Views.CloseMode.Kill:
					var tmpThis = this;
					var firstOrDefault = System.Diagnostics.Process.GetProcesses().FirstOrDefault(p=>p.MainModule.FileName == tmpThis.Module);
					if (firstOrDefault != null)
						firstOrDefault.Kill();
					break;
				case Views.CloseMode.None:
					break;
				default:
					throw new ArgumentOutOfRangeException("closeMode", closeMode, null);
			}
		}

		public override int GetHashCode(){
			unchecked{
				return ((Title != null ? Title.GetHashCode() : 0)*397) ^ (Module != null ? Module.GetHashCode() : 0);
			}
		}

		public override string ToString(){
			return Title + " - " + Module;
		}
	}
}