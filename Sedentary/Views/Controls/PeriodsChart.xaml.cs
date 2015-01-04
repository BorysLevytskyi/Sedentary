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

			DataContext = this;

			TimeScale = TimeSpan.FromHours(8);
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
			const int chunkSizeInSeconds = 30*60; // 30 min
			int periodsTotalInSeconds = (int)WorkPeriods.Sum(p => p.Length.TotalSeconds);

		    int chunksCount = (int)Math.Ceiling((double) periodsTotalInSeconds/(double) chunkSizeInSeconds);

			TimeScale = TimeSpan.FromSeconds(Math.Max(chunkSizeInSeconds, chunksCount * chunkSizeInSeconds));
		}
	}
}