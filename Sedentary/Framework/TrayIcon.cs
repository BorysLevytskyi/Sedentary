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
		private IconConfig _cfg;

		private NotifyIcon _icon;

		public TrayIcon(Statistics stats)
		{
			_stats = stats;
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
			_icon.ShowBalloonTip(5000, "Reminder", string.Format(@"You've been sitting for {0:h\h\ m\m}.", _stats.SessionTime),
				ToolTipIcon.Warning);
		}

		public void Refresh()
		{
			if (_stats.IsSitting)
			{
				int overlayHeight = (int) Math.Floor(16d *_stats.SittingTimeCompletionRate);

				Icon =
					Icon.SetOverlay(Color.Red, overlayHeight)
						.SetWorkState(_stats.CurrentPeriod.State);
			}
			else
			{
				Icon = Icon.SetOverlay(Icon.OverlayColor, 0).SetWorkState(_stats.CurrentPeriod.State);
			}
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
			_icon.Text = state + " for " + _stats.SessionTime.ToString(@"h\h\ m\m");
		}
	}
}