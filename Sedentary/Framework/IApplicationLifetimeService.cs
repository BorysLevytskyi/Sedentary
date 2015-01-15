using Sedentary.Model;
using Sedentary.Model.Persistence;

namespace Sedentary.Framework
{
	internal class ApplicationLifetimeService
	{
		private readonly WorkTracker _workTracker;
		private readonly Statistics _stats;

		public ApplicationLifetimeService(WorkTracker workTracker, Statistics stats)
		{
			_workTracker = workTracker;
			_stats = stats;
		}

		public void OnStart()
		{
			_workTracker.Start();
		}

		public void OnExit()
		{
			StatsRepo.Save(_stats);
		}
	}
}