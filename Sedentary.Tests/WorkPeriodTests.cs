using System;
using FluentAssertions;
using NUnit.Framework;
using Sedentary.Model;

namespace Sedentary.Tests
{
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