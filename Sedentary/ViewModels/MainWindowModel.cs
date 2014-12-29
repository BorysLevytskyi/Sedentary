using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sedentary.Framework;
using Sedentary.Framework.Diagnostics;
using Sedentary.Model;
using Sedentary.Properties;

namespace Sedentary.ViewModels
{
	public class MainWindowModel : INotifyPropertyChanged
	{
		private readonly Statistics _stats;
		private readonly Analyzer _analyzer;
		private StringBuilder _traceOutput;

		public MainWindowModel(Statistics stats, Analyzer analyzer)
		{
			_stats = stats;
			_analyzer = analyzer;
			_stats.Changed += Refresh;
			_stats.Periods.CollectionChanged += (s, o) => OnPropertyChanged("Periods");
		}

		public bool IsSitting
		{
			get { return _stats.IsSitting; }
		}

		public string TotalSittingTime
		{
			get { return _stats.TotalSittingTime.ToString(@"h\h\ m\m"); }
		}

		public WorkPeriod CurrentPeriod
		{
			get { return _stats.CurrentPeriod; }
		}

		public List<WorkPeriod> Periods
		{
			get { return _stats.Periods.ToList(); }
		}

		public bool IsSittingLimitExceeded
		{
			get { return _analyzer.IsSittingLimitExceeded; }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void Refresh()
		{
			OnPropertyChanged("TotalSittingTime");
			OnPropertyChanged("CurrentPeriod");
			OnPropertyChanged("IsSittingLimitExceeded");
			OnPropertyChanged("Status");
		}

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}