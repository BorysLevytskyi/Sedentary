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

		public MainWindowModel(Statistics stats)
		{
			_stats = stats;
			_stats.Changed += Refresh;
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

		public IEnumerable<WorkPeriod> Periods
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
		}

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}