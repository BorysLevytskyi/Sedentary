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
		private IContainer _container;

		public AppBootstrapper()
		{
			Initialize();
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
			_container = builder.Build();
		}

		public static void RegisterContainer(ContainerBuilder builder)
		{
			builder.Register(c => Requirements.Create()).AsSelf().SingleInstance();
			builder.Register(c => StatsRepo.Get()).AsSelf().SingleInstance();
			builder.RegisterType<Analyzer>().SingleInstance();
			builder.RegisterType<IdleWatcher>().SingleInstance();
			builder.RegisterType<TrayIcon>().SingleInstance();
			builder.RegisterType<WorkTracker>().SingleInstance();
			builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
			builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
			builder.RegisterType<ShellViewModel>().AsSelf().PropertiesAutowired().SingleInstance();
			builder.RegisterType<PeriodsChartViewModel>().AsSelf().SingleInstance();
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			return string.IsNullOrEmpty(key) ? _container.Resolve(serviceType) : _container.ResolveKeyed(key, serviceType);
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			var t1 = typeof (IEnumerable<>).MakeGenericType(serviceType);
			var resolve = (IEnumerable)_container.Resolve(t1);
			return resolve.Cast<object>();
		}

		protected override void BuildUp(object instance)
		{
			_container.InjectProperties(instance);
		}
	}
}