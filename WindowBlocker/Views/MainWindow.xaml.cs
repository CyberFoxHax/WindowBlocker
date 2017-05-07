using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace WindowBlocker.Views {
	public partial class MainWindow {
		public MainWindow(){
			InitializeComponent();

			Hide();

			if (System.IO.File.Exists(FileName))
				ReadFile();

			new System.Threading.Thread(p =>{
				System.Threading.Thread.Sleep(15000);
				_backgroundPoller = new BackgroundPoller(_deathRowWindows);
				Dispatcher.BeginInvoke(new System.Action(() => {
					Background = System.Windows.Media.Brushes.YellowGreen;
				}));
			}).Start();
		}

		private readonly List<WindowInfo> _deathRowWindows = new List<WindowInfo>();
		private BackgroundPoller _backgroundPoller;

		private static readonly string FileName = System.Environment.CurrentDirectory + "\\blockList.json";

		private void BtnSave_OnClick(object sender, RoutedEventArgs e){
			_deathRowWindows.Clear();
			foreach (var windowInfo in WindowList.ExistingList)
				_deathRowWindows.Add(windowInfo);

			SaveFile();
		}

		private void SaveFile(){
			var str = Newtonsoft.Json.JsonConvert.SerializeObject(_deathRowWindows, Newtonsoft.Json.Formatting.Indented);
			System.IO.File.WriteAllText(FileName, str);
		}

		private void ReadFile(){
			var text = System.IO.File.ReadAllText(FileName, System.Text.Encoding.UTF8);
			List<WindowInfo> res;
			try{
				res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WindowInfo>>(text);
			}
			catch (System.Exception){
				MessageBox.Show("Unable to read blocklist");
				return;
			}

			if (res == null || res.Count == 0)
				return;

			foreach (var windowInfo in res) {
				_deathRowWindows.Add(windowInfo);
				WindowList.ExistingList.Add(windowInfo);
				WindowList.RenderExistingList();
			}
		}

		private void BtnClose_OnClick(object sender, RoutedEventArgs e){
			Hide();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e){
			Hide();
			e.Cancel = true;
			base.OnClosing(e);
		}

		protected override void OnClosed(System.EventArgs e){
			base.OnClosed(e);
			WindowList.Dispose();
			Taskbar.NotifyIcon.Dispose();
			if (_backgroundPoller != null) _backgroundPoller.Dispose();
		}
	}
}
