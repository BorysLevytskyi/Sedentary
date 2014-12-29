using System;

namespace Sedentary.Model
{
	public class Requirements
	{
		public TimeSpan MaxSittingTime { get; set; }

		public TimeSpan AwayThreshold { get; set; }

		public TimeSpan RequiredRestingTime { get; set; }
	}
}