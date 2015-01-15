using System;
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

			double accumulatedSittingPressureRate = 
				Math.Min(sittingPeriod.Length.GetCompletionRateFor(_requirements.MaxSittingTime), 1); // Pressure rate that was accumulated previously sittin

			var restingRate = timeResting.GetCompletionRateFor(_requirements.RequiredRestingTime);

			if ((int) restingRate == 1)
			{
				return 0;
			}

			double pressureRate = 1 - restingRate;
			pressureRate = pressureRate*accumulatedSittingPressureRate;


			return Math.Max(0, pressureRate);
		}

		public double GetPressureRatio()
		{
			double pressureRate = 0;
			foreach (var period in _stats.Periods)
			{
				if (period.State == WorkState.Sitting)
				{
					pressureRate = (pressureRate + period.Length.GetCompletionRateFor(_requirements.MaxSittingTime)).InRangeOf(0,1);
				}
				else // Resting period
				{
					double restingRate = period.Length.GetCompletionRateFor(_requirements.RequiredRestingTime);
					double subTraction = pressureRate = 1 - restingRate;
					subTraction = subTraction*pressureRate;
					pressureRate = (pressureRate - subTraction).InRangeOf(0, 1);
				}
			}

			return Math.Round(pressureRate, 2);
		}
	}
}