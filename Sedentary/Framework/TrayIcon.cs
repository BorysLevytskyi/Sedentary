using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Sedentary.Model;
using Application = System.Windows.Application;

namespace Sedentary.Framework
{
	public interface ITrayIcon : IDisposable
	{
		void Init();
		void ShowWarning(string message, TimeSpan timeOut);
		void SetIcon(WorkState state, double pressureRatio);
	}

	public class TrayIcon : ITrayIcon
	{
		private IconConfig _cfg;

		private NotifyIcon _icon;

		public TrayIcon()
		{
			_cfg = new IconConfig(WorkState.Sitting);
		}

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

		public void ShowWarning(string message, TimeSpan timeout)
		{
			_icon.ShowBalloonTip((int) timeout.TotalMilliseconds, "Reminder",
				string.Format(@"You've been sitting for {0:h\h\ m\m}.", _stats.CurrentPeriodLength),
				ToolTipIcon.Warning);
		}

		public void SetIcon(WorkState state, double pressureRatio)
		{
			Color color = GetColor(pressureRatio);
			Icon = Icon.SetWorkState(state).SetOverlay(color, (int) Math.Floor(16d*pressureRatio));
		}

		public void Init()
		{
			_icon = new NotifyIcon {Visible = true};

			UpdateIcon();

			_icon.MouseClick += OnIconOnClick;
			_icon.MouseMove += OnIconMouseMove;
		}

		public event Action OnPositionSwitch;

		private void UpdateIcon()
		{
			if (_icon.Icon != null)
			{
				_icon.Icon.Dispose();
			}

			Bitmap bitmap = IconProvider.GetIconImage(_cfg);

			_icon.Icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
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
			string state = _stats.CurrentPeriod.State.ToString();
			_icon.Text = state + " for " + _stats.CurrentPeriodLength.ToString(@"h\h\ m\m");
		}
	}
}