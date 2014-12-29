using System;
using System.Diagnostics;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class Analyzer
	{
		private readonly Statistics _stats;
		private readonly Requirements _requirements;

		public Analyzer(Statistics stats, Requirements requirements)
		{
			_stats = stats;
			_requirements = requirements;
		}

		public bool IsSittingLimitExceeded
		{
			get
			{
				return _stats.CurrentState == WorkState.Sitting && _stats.CurrentPeriodLength >= _requirements.MaxSittingTime;
			}
		}

		// TODO: Remove trace one proven to work
		public double GetSittingPressureRate()
		{
			if (_stats.IsSitting)
			{
				return _stats.CurrentPeriod.Length.GetCompletionRateFor(_requirements.MaxSittingTime);
			}

			if (_stats.PreviousPeriod == null || _stats.PreviousPeriod.State != WorkState.Sitting)
			{
				return 0;
			}

			var sittingPeriod = _stats.PreviousPeriod;
			var timeResting = DateTime.Now.TimeOfDay - sittingPeriod.EndTime;

			double accumulatedSittingPressureRate = Math.Max(sittingPeriod.Length.GetCompletionRateFor(_requirements.MaxSittingTime), 1); // Pressure rate that was accumulated previously sitting
			double restingToSittingRate = Math.Round((double)_requirements.RequiredRestingTime.Ticks / _requirements.MaxSittingTime.Ticks, 2);

			Trace.Indent();
			Tracer.Write("Resting to sitting rate is: {0}", restingToSittingRate);

			var restingRate = timeResting.GetCompletionRateFor(_requirements.RequiredRestingTime);

			if ((int) restingRate == 1)
			{
				Trace.Unindent();
				return 0;
			}

			Tracer.Write("Resting rate = {0}", restingRate);

			double pressureRate = 1 - restingRate;

			Tracer.Write("Pressure rate = {0}", pressureRate);

			pressureRate = pressureRate*accumulatedSittingPressureRate;

			Tracer.Write("Pressure rate adjusted by sitting pressure {0} = {1}", accumulatedSittingPressureRate, pressureRate);

			Trace.Unindent();

			return Math.Max(0, pressureRate);
		}
	}
}