using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Caliburn.Micro;
using Sedentary.Framework;
using Sedentary.Model;
using Sedentary.Model.Persistence;
using Sedentary.ViewModels;
using Parameter = Autofac.Core.Parameter;

namespace Sedentary
{
	public class AppBootstrapper : BootstrapperBase
	{
		private ILifetimeScope _scope;

		public AppBootstrapper()
		{
			Initialize();
		}

		protected override void OnExit(object sender, EventArgs e)
		{
			base.OnExit(sender, e);

			_scope.Resolve<ApplicationLifetimeService>().OnExit();
			_scope.Dispose();
		}

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			base.OnStartup(sender, e);

			DisplayRootViewFor<ShellViewModel>();
		}

		protected override void Configure()
		{
			base.Configure();

			var builder = new ContainerBuilder();
			RegisterContainer(builder);

			_scope = builder.Build().BeginLifetimeScope();
		}

		public static void RegisterContainer(ContainerBuilder builder)
		{
			builder.RegisterType<ApplicationLifetimeService>().AsSelf();
			builder.Register(c => Requirements.Create()).AsSelf().InstancePerLifetimeScope();
			builder.Register(c => StatsRepo.Get()).AsSelf().InstancePerLifetimeScope();
			builder.RegisterType<Analyzer>().InstancePerLifetimeScope();
			builder.RegisterType<IdleWatcher>().InstancePerLifetimeScope();
			builder.RegisterType<TrayIcon>().InstancePerLifetimeScope();
			builder.RegisterType<WorkTracker>().InstancePerLifetimeScope();
			builder.RegisterType<WindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
			builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();
			builder.RegisterType<ShellViewModel>().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
			builder.RegisterType<PeriodsChartViewModel>().AsSelf().InstancePerLifetimeScope();
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			return string.IsNullOrEmpty(key) ? _scope.Resolve(serviceType) : _scope.ResolveKeyed(key, serviceType);
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			var t1 = typeof (IEnumerable<>).MakeGenericType(serviceType);
			var resolve = (IEnumerable)_scope.Resolve(t1);
			return resolve.Cast<object>();
		}

		protected override void BuildUp(object instance)
		{
			_scope.InjectProperties(instance);
		}
	}
}