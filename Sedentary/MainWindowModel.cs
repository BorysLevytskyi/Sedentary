using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using SittingTracker.Model;
using SittingTracker.Properties;

namespace SittingTracker
{
	public class MainWindowModel : INotifyPropertyChanged
	{
		private readonly Statistics _stats;
		private readonly BindingList<WorkPeriod> _periods;

		public MainWindowModel(Statistics stats)
		{
			_stats = stats;
			_stats.Changed += Refresh;
			_periods = new BindingList<WorkPeriod>();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsSitting
		{
			get { return _stats.IsSitting; }
		}

		public bool IsStanding
		{
			get { return !IsSitting; }
		}

		public string TotalSittingTime
		{
			get { return _stats.TotalSittingTime.ToString(@"h\h\ m\m"); }
		}

		public WorkPeriod CurrentPeriod
		{
			get { return _stats.CurrentPeriod; }
		}

		public IEnumerable Periods
		{
			get { return _stats.Periods; }
		}

		public bool IsSittingLimitExceeded
		{
			get { return _stats.IsSittingLimitExceeded; }
		}

		public void Refresh()
		{
			OnPropertyChanged("TotalSittingTime");
			OnPropertyChanged("CurrentPeriod");
			OnPropertyChanged("IsSittingLimitExceeded");
			OnPropertyChanged("Status");
			OnPropertyChanged("Periods");

			// TODO: Redo this
			foreach (var p in _stats.Periods.Where(p => !_periods.Contains(p)))
			{
				_periods.Add(p);
			}
		}

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}