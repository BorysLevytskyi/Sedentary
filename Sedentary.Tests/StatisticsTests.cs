using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sedentary.Model;
using Sedentary.Properties;

namespace Sedentary.Tests
{
	[TestFixture]
	public class StatisticsTests
	{
		[Test]
		public void ShouldMergePeriods()
		{
			WorkPeriod target;
			var h1 = TimeSpan.FromHours(1);
			var h2 = TimeSpan.FromHours(2);
			var h3 = TimeSpan.FromHours(3);
			var h4 = TimeSpan.FromHours(4);

			WorkPeriod[] periods =
			{
				new WorkPeriod(WorkState.Sitting, h1, h2), 
				target = new WorkPeriod(WorkState.Standing, h2, h3), 
				new WorkPeriod(WorkState.Sitting, h3),  // running
			};

			var stats = new Statistics(periods);
			stats.ChangePeriodState(target, WorkState.Sitting);
			stats.Periods.Should().HaveCount(1);

			var period = stats.Periods.First();

			period.State.Should().Be(WorkState.Sitting);
			period.StartTime.Should().Be(h1);
			period.IsCompleted.Should().BeFalse();
		}
	}

	[TestFixture]
	public class WorkPeriodTests
	{
		[Test]
		public void EndTimeLessThanStartTime()
		{
			var p = new WorkPeriod(WorkState.Sitting, TimeSpan.FromHours(4), TimeSpan.FromHours(1));
			p.Length.Should().Be(TimeSpan.FromHours(21));
		}
	}
}