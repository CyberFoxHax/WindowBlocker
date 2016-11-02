using System.Linq;
using System.Collections.Generic;

namespace WindowBlocker.Views{
	public class BackgroundPoller{
		public BackgroundPoller(List<WindowInfo> deathRow){
			_deathRow = deathRow;
			_timer.Elapsed += TimerOnElapsed;
			_timer.Start();
		}

		private readonly List<WindowInfo> _deathRow;
		private readonly List<WindowInfo> _closedWindows = new List<WindowInfo>();

		private readonly System.Timers.Timer _timer = new System.Timers.Timer(150){AutoReset = false};

		private void TimerOnElapsed(object sender, System.Timers.ElapsedEventArgs elapsedEventArgs){
			var activeWindows = Interop.WindowGetter.GetDesktopWindows();
			var indexedDeathRow = _deathRow.Select(p=>new{
				Interop = activeWindows.FirstOrDefault(pp => p==pp),
				View = p
			}).ToArray();

			var activeWindowsOnDeathRow = indexedDeathRow.Where(p => p.Interop.Valid).ToArray();

			var closeAlwaysWindows = (
				from window in activeWindowsOnDeathRow
				where window.View.CloseCount == CloseCount.Always
				select window
			).ToArray();
			foreach (var newWindow in closeAlwaysWindows)
				newWindow.Interop.CloseWindow(newWindow.View.CloseMode);

			var closeOnce = (
				from window in activeWindowsOnDeathRow
				where window.View.CloseCount == CloseCount.Once
				&& _closedWindows.Contains(window.View) == false
				select window
			).ToArray();
			foreach (var windowInfo in closeOnce){
				_closedWindows.Add(windowInfo.View);
				windowInfo.Interop.CloseWindow(windowInfo.View.CloseMode);
			}

			var removedFromCloseOnce = (
				from window in indexedDeathRow
				where window.View.CloseCount == CloseCount.Always
				&& _closedWindows.Contains(window.View)
				select window
			).ToArray();
			foreach (var removed in removedFromCloseOnce)
				_closedWindows.Remove(removed.View);

			_timer.Start();
		}

		public void Dispose(){
			_timer.Elapsed -= TimerOnElapsed;
			_timer.Stop();
		}
	}
}