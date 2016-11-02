using System;
using System.Windows;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace WindowBlocker.Views {
	public partial class WindowButton {

		public WindowButton() {
			InitializeComponent();

			_closeCountRadioButtons = new Dictionary<CloseCount, ToggleButton>{
				{CloseCount.Always, RadioButtonAlways},
				{CloseCount.Once, RadioButtonOnce}
			};

			_closeModeRadioButtons = new Dictionary<CloseMode, ToggleButton>{
				{CloseMode.Close, RadioButtonClose},
				{CloseMode.Kill, RadioButtonKill},
				{CloseMode.None, RadioButtonNone}
			};

			Background = null;
		}

		private WindowInfo _windowInfo;

		private readonly Dictionary<CloseCount, ToggleButton> _closeCountRadioButtons;
		private readonly Dictionary<CloseMode, ToggleButton> _closeModeRadioButtons;

		public event Action<WindowButton> Change;

		private static void SetGroupNames(string name, params System.Windows.Controls.RadioButton[] controls){
			foreach (var radioButton in controls)
				radioButton.GroupName += name;
		}

		public WindowInfo WindowInfo
		{
			get { return _windowInfo; }
			set {
				_windowInfo = value;
				TxtTitle.Text = _windowInfo.Title;
				TxtModule.Text = _windowInfo.Module;

				SetGroupNames(GetType().Name + _windowInfo.Title + _windowInfo.Module,
					RadioButtonAlways,
					RadioButtonClose,
					RadioButtonKill,
					RadioButtonNone,
					RadioButtonOnce
				);

				foreach (var btn in _closeCountRadioButtons)
					btn.Value.IsChecked = btn.Key == value.CloseCount;

				foreach (var btn in _closeModeRadioButtons)
					btn.Value.IsChecked = btn.Key == value.CloseMode;

				CheckBoxTitle .IsChecked = value.UseTitle;
				CheckBoxModule.IsChecked = value.UseModule;
			}
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e){
			var radioSender = sender as ToggleButton;
			if(radioSender == null) return;

			CloseCount? closeCount = null;
			CloseMode? closeMode = null;

			if (_closeCountRadioButtons.ContainsValue(radioSender))
				closeCount = _closeCountRadioButtons.First(p => p.Value == radioSender).Key;

			else if (_closeModeRadioButtons.ContainsValue(radioSender))
				closeMode = _closeModeRadioButtons.First(p => p.Value == radioSender).Key;

			if ((closeCount == null || closeCount == _windowInfo.CloseCount) &&
				(closeMode == null || closeMode == _windowInfo.CloseMode)) return;

			if (closeCount != null) _windowInfo.CloseCount = closeCount.Value;
			if (closeMode  != null) _windowInfo.CloseMode  = closeMode.Value ;

			if (Change != null)
				Change(this);
		}

		private void CheckBox_OnChange(object sender, RoutedEventArgs e){
			var checkbox = sender as System.Windows.Controls.CheckBox;
			if (checkbox == null || _windowInfo == null) return;

			if (checkbox == CheckBoxTitle) {
				if (_windowInfo.UseTitle != (CheckBoxTitle.IsChecked == true)) {
					_windowInfo.UseTitle = CheckBoxTitle.IsChecked == true;

					if (Change != null)
						Change(this);
				}
			}

			if (checkbox == CheckBoxModule) {
				if (_windowInfo.UseModule != (CheckBoxModule.IsChecked == true)) {
					_windowInfo.UseModule = CheckBoxModule.IsChecked == true;

					if (Change != null)
						Change(this);
				}
			}
		}
	}
}
