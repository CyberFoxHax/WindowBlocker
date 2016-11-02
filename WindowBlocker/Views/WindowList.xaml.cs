using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WindowBlocker.Views {
	public partial class WindowList {
		public WindowList(){
			InitializeComponent();

			Loaded += OnLoaded;

			_timer = new System.Timers.Timer(1000){
				AutoReset = false
			};
			_timer.Elapsed += OnTimerOnElapsed;
			_timer.Start();

			this.MouseEnter += (sender, args) => _timer.Stop();
			this.MouseLeave += (sender, args) => _timer.Start();

			DependencyPropertyChangedEventHandler evt = (sender, args) => {
				if (IsVisible)
					_timer.Start();
				else
					_timer.Stop();
			};
			evt(null, new DependencyPropertyChangedEventArgs());
			IsVisibleChanged += evt;

			for (var i = 0; i < _windowButtons.Length; i++)
				_windowButtons[i] = new ButtonCache();
		}

		private void OnTimerOnElapsed(object sender, System.Timers.ElapsedEventArgs args){
			Dispatcher.BeginInvoke(new System.Action(() =>{
				RenderNewList();
				_timer.Start();
			}));
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs){
			Render();
		}

		public void Render(){
			RenderExistingList();
			RenderNewList();
		}

		public static List<WindowInfo> ExistingList = new List<WindowInfo>();
		private readonly System.Timers.Timer _timer;

		public void RenderExistingList(){
			foreach (var button in _windowButtons.Where(p => p.Alive && p.ExistingWindow)) {
				button.Alive = false;
				button.Button.Change -= WindowButtonClick;
			}
			ExistingWindows.Children.Clear();
			foreach (var window in ExistingList){
				var btn = ButtonCache.GetNew(_windowButtons, true);
				btn.WindowInfo = window;
				btn.Change += WindowButtonClick;
				ExistingWindows.Children.Add(btn);
			}
		}

		private class ButtonCache{
			public bool Alive;
			public bool ExistingWindow;
			public readonly WindowButton Button;

			public ButtonCache(){
				Button = new WindowButton();
				Alive = false;
			}

			public static WindowButton GetNew(IEnumerable<ButtonCache> cache, bool isExistingContainer){
				var btn = cache.FirstOrDefault(p => p.Alive == false);
				if (btn == null) return null;
				btn.ExistingWindow = isExistingContainer;
				btn.Alive = true;
				return btn.Button;
			}
		}

		private readonly ButtonCache[] _windowButtons = new ButtonCache[50];

		public void RenderNewList(){
			foreach (var button in _windowButtons.Where(p => p.Alive && p.ExistingWindow == false)) {
				button.Alive = false;
				button.Button.Change -= WindowButtonClick;
			}
			NewWindows.Children.Clear();
			var activeWindows = Interop.WindowGetter.GetDesktopWindows();
			var indexedDeathRow = ExistingList.Select(p => new {
				Interop = activeWindows.FirstOrDefault(pp => p == pp),
				View = p
			}).ToArray();

			foreach (var activeWindow in activeWindows.ToArray().Where(p => indexedDeathRow.Select(pp=>pp.Interop).Contains(p)))
				activeWindows.Remove(activeWindow);

			foreach (var window in activeWindows){
				var btn = ButtonCache.GetNew(_windowButtons, false);
				btn.WindowInfo = new WindowInfo(window);
				btn.Change += WindowButtonClick;
				NewWindows.Children.Add(btn);
			}
		}

		private void WindowButtonClick(WindowButton obj){
			if (obj.WindowInfo.CloseMode != CloseMode.None && NewWindows.Children.Contains(obj)){
				ExistingList.Add(obj.WindowInfo);

				NewWindows.Children.Remove(obj);
				ExistingWindows.Children.Add(obj);
				_windowButtons.First(p => p.Button == obj).ExistingWindow = true;
			}
			else if (obj.WindowInfo.CloseMode == CloseMode.None && ExistingWindows.Children.Contains(obj)) {
				ExistingList.Remove(obj.WindowInfo);

				ExistingWindows.Children.Remove(obj);
				NewWindows.Children.Add(obj);
				_windowButtons.First(p => p.Button == obj).ExistingWindow = false;
			}
		}

		public void Dispose(){
			_timer.Elapsed -= OnTimerOnElapsed;
			_timer.Stop();
		}
	}
}
