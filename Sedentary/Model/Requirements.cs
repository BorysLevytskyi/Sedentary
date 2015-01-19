using System;
using System.Configuration;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class Requirements
	{
		public TimeSpan MaxSittingTime { get; set; }

		public TimeSpan AwayThreshold { get; set; }

		public TimeSpan RequiredRestingTime { get; set; }

		public static Requirements Create()
		{
			try
			{
				var awayThreshold = TimeSpan.Parse(ConfigurationManager.AppSettings["awayThreshold"]);
				var maxSittingTime = TimeSpan.Parse(ConfigurationManager.AppSettings["maxSittingTime"]);
				var restingPeriod = TimeSpan.Parse(ConfigurationManager.AppSettings["requiredRestingTime"]);

			    return new Requirements
			    {
			        AwayThreshold = awayThreshold,
			        MaxSittingTime = maxSittingTime,
			        RequiredRestingTime = restingPeriod
			    };
			}
			catch (Exception ex)
			{
				Tracer.WriteError("Failed to load requirements from config", ex);
				return CreateDefault();
			}
		}

		private static Requirements CreateDefault()
		{
			return new Requirements
			{
				AwayThreshold = TimeSpan.FromMinutes(5),
				MaxSittingTime = TimeSpan.FromHours(1),
				RequiredRestingTime = TimeSpan.FromMinutes(5)
			};
		}
	}
}