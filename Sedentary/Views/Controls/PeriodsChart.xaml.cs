using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Sedentary.Framework;
using Sedentary.Model;

namespace Sedentary.Views.Controls
{
	/// <summary>
	///     Interaction logic for PeriodsChart.xaml
	/// </summary>
	public partial class PeriodsChart : UserControl
	{
		public static readonly DependencyProperty TimeScaleProperty = DependencyProperty.Register(
			"TimeScale", typeof (TimeSpan), typeof (PeriodsChart), new PropertyMetadata(TimeSpan.Zero));

		public static readonly DependencyProperty WorkPeriodsProperty = DependencyProperty.Register(
			"WorkPeriods", typeof (IList<WorkPeriod>), typeof (PeriodsChart),
			new PropertyMetadata(default(IList<WorkPeriod>), (sender, args) => ((PeriodsChart) sender).OnPeriodsChanged(args)));

		public PeriodsChart()
		{
			InitializeComponent();

			this.DataContext = this;

			TimeScale = TimeSpan.FromHours(8);
		}

		private IEnumerable<WorkPeriod> CreateDesignPeriods()
		{
			yield return new WorkPeriod(WorkState.Sitting, TimeSpan.Zero, TimeSpan.FromHours(1));
			yield return new WorkPeriod(WorkState.Standing, TimeSpan.FromHours(1), TimeSpan.FromHours(2));
			yield return new WorkPeriod(WorkState.Away, TimeSpan.FromHours(2), TimeSpan.FromHours(4));
			yield return new WorkPeriod(WorkState.Sitting, TimeSpan.FromHours(4), TimeSpan.FromHours(6));
		}

		public TimeSpan TimeScale
		{
			get { return (TimeSpan) GetValue(TimeScaleProperty); }
			set { SetValue(TimeScaleProperty, value); }
		}

		public IList<WorkPeriod> WorkPeriods
		{
			get { return (IList<WorkPeriod>)GetValue(WorkPeriodsProperty); }
			set { SetValue(WorkPeriodsProperty, value); }
		}

		private void OnPeriodsChanged(DependencyPropertyChangedEventArgs args)
		{
			Tracer.Write("Periods changed");
		}
	}
}