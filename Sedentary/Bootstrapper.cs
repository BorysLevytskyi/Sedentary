using System.Windows;
using Autofac;
using Caliburn.Micro;
using Sedentary.Framework;
using Sedentary.Model;
using Sedentary.Model.Persistence;
using Sedentary.ViewModels;

namespace Sedentary
{
	public class Bootstrapper : BootstrapperBase
	{
		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			base.OnStartup(sender, e);

			DisplayRootViewFor<MainWindowModel>();
		}

		public static void RegisterContainer(ContainerBuilder builder)
		{
			builder.Register(c => Requirements.Create()).AsSelf();
			builder.Register(c => StatsRepo.Get()).AsSelf();
			builder.RegisterType<Analyzer>();
			builder.RegisterType<IdleWatcher>();
			builder.RegisterType<TrayIcon>();
			builder.RegisterType<WorkTracker>();
		}
	}
}