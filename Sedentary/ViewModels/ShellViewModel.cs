using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Sedentary.Model;

namespace Sedentary.ViewModels
{
	public class ShellViewModel : PropertyChangedBase
	{
		private readonly Analyzer _analyzer;
		private readonly Statistics _stats;

		public ShellViewModel(Statistics stats, Analyzer analyzer)
		{
			_stats = stats;
			_analyzer = analyzer;
			_stats.Changed += Refresh;
			_stats.Periods.CollectionChanged += (s, o) => Refresh();
		}

		public PeriodsChartViewModel Chart { get; set; }

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

		public double PressureRatio
		{
			get { return _analyzer.GetPressureRatio(); }
		}

		public void ClearStatistics()
		{
			_stats.Clear();
		}
	}
}