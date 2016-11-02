namespace WindowBlocker.Views{
	public class WindowInfo{
		public WindowInfo(){
			UseTitle = true;
			UseModule = true;
		}

		public WindowInfo(Interop.WindowInfo p){
			Title = p.Title;
			Module = p.Module;
		}

		public string Title { get; set; }
		public string Module { get; set; }
		public CloseCount CloseCount { get; set; }
		public CloseMode CloseMode { get; set; }
		public bool UseTitle { get; set; }
		public bool UseModule { get; set; }

		public static bool operator ==(WindowInfo a, Interop.WindowInfo b){
			var aIsNull = ReferenceEquals(a, null);
			var bIsNull = ReferenceEquals(b, null);
			return
					aIsNull && bIsNull
				|| (
					aIsNull == false && bIsNull == false
				&& (a.UseModule == false || a.Module == b.Module)
				&& (a.UseTitle == false  || a.Title == b.Title)
			);
		}

		public static bool operator !=(WindowInfo a, Interop.WindowInfo b){
			return !(a == b);
		}	

		protected bool Equals(WindowInfo other) {
			return string.Equals(Title, other.Title) && string.Equals(Module, other.Module);
		}

		protected bool Equals(Interop.WindowInfo other) {
			return this == other;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((WindowInfo)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((Title != null ? Title.GetHashCode() : 0) * 397) ^ (Module != null ? Module.GetHashCode() : 0);
			}
		}

		public override string ToString() {
			return Title + " - " + Module;
		}
	}
}