using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using SittingTracker.Model;
using Application = System.Windows.Application;

namespace SittingTracker.Framework
{
	public class TrayIcon
	{
		private readonly Statistics _stats;

		public TrayIcon(Statistics stats)
		{
			_stats = stats;
		}

		private NotifyIcon _icon;

		public void UpdateIcon()
		{
			if (_icon.Icon != null)
			{
				_icon.Icon.Dispose();
			}

			var image = new Bitmap(GetIconPath());
			_icon.Icon = Icon.FromHandle(image.GetHicon());

			UpdateTooltip();

			Tracer.TraceMe("Icon updated");
		}

	    public void ShowWarning()
	    {
	        _icon.ShowBalloonTip(5000, "Reminder", string.Format(@"You've been sitting for {0:h\h\ m\m}.", _stats.SessionTime), ToolTipIcon.Warning);
	    }

		private string GetIconPath()
		{
			if (_stats.IsUserAway)
			{
				return "away.png";
			}

			if (_stats.IsSitting)
			{
				return _stats.IsSittingLimitExceeded ? "sitting_bad.png" : "sitting.png";
			}

			return "standing.jpg";
		}

		public void Init()
		{
			_icon = new NotifyIcon {Visible = true};

			UpdateIcon();

			_icon.MouseClick += OnIconOnClick;
			_icon.MouseMove += OnIconMouseMove;
		}

		void OnIconMouseMove(object sender, MouseEventArgs e)
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

		public event Action OnPositionSwitch;

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