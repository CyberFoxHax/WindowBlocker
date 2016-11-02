using System.Windows;

namespace WindowBlocker.Views {
	public partial class Taskbar {
		public Taskbar(){
			InitializeComponent();
		}

		private bool _visible;

		private void Open_OnClick(object sender, RoutedEventArgs e){
			_visible = true;
			this.GetParentWindow().Show();
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e){
			new System.Threading.Thread(() => {
				Application.Current.Dispatcher.Invoke(new System.Action(Application.Current.Shutdown));
			}).Start();
		}

		private void Taskbar_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e){
			if (_visible = !_visible)
				this.GetParentWindow().Hide();
			else
				this.GetParentWindow().Show();
		}
	}
}
