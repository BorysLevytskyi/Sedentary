using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Sedentary.Model;
using Application = System.Windows.Application;

namespace Sedentary.Framework
{
	public class TrayIcon : IDisposable
	{
		private readonly Statistics _stats;
		private readonly Analyzer _analyzer;
		private IconConfig _cfg;

		private NotifyIcon _icon;

		public TrayIcon(Statistics stats, Analyzer analyzer)
		{
			_stats = stats;
			_analyzer = analyzer;
		}

		public event Action OnPositionSwitch;

		private IconConfig Icon
		{
			get { return _cfg; }
			set
			{
				if (_cfg.GetHashCode() == value.GetHashCode())
				{
					return;
				}

				_cfg = value;

				UpdateIcon();
			}
		}

		public void Dispose()
		{
			if (_icon != null)
			{
				_icon.Dispose();
				_icon = null;
			}
		}

		private void UpdateIcon()
		{
			if (_icon.Icon != null)
			{
				_icon.Icon.Dispose();
			}

			Bitmap bitmap = IconProvider.GetIconImage(_cfg);

			_icon.Icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());

			Tracer.Write("Icon updated");
		}

		public void ShowWarning()
		{
			_icon.ShowBalloonTip(5000, "Reminder", string.Format(@"You've been sitting for {0:h\h\ m\m}.", _stats.CurrentPeriodLength),
				ToolTipIcon.Warning);
		}

		public void Refresh()
		{
			var pressureRate = _analyzer.GetSittingPressureRate();
			int overlayHeight = (int)Math.Floor(16d * pressureRate);
			var color = GetColor(pressureRate);

			Icon = Icon.SetOverlay(color, overlayHeight).SetWorkState(_stats.CurrentState);
		}

		private Color GetColor(double rate)
		{
			if (rate <= 0.5d)
			{
				return Color.Green;
			}

			if (rate <= 0.8d)
			{
				return Color.Orange;
			}

			return Color.Red;
		}

		public void Init()
		{
			_icon = new NotifyIcon {Visible = true};

			UpdateIcon();

			_icon.MouseClick += OnIconOnClick;
			_icon.MouseMove += OnIconMouseMove;
		}

		private void OnIconMouseMove(object sender, MouseEventArgs e)
		{
			UpdateTooltip();
		}

		private void OnIconOnClick(object s, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				OnSittingSwitch();
			}
			else
			{
				Window form = Application.Current.MainWindow;

				if (form.IsVisible)
				{
					form.Hide();
				}
				else
				{
					form.Show();
					form.Activate();
				}
			}
		}

		protected virtual void OnSittingSwitch()
		{
			Action handler = OnPositionSwitch;
			if (handler != null) handler();
		}

		private void UpdateTooltip()
		{
			string state = _stats.IsSitting ? "Sitting" : "Standing";
			_icon.Text = state + " for " + _stats.CurrentPeriodLength.ToString(@"h\h\ m\m");
		}
	}
}