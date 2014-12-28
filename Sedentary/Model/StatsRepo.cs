using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Serialization;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public static class StatsRepo
	{
		public static Statistics Get()
		{
			return new Statistics();

			if (!File.Exists(FilePath.Value))
			{
				Tracer.Write("Not found previously saved statistics. New statistics created.");
				return new Statistics();
			}

			PersistentStats saved;

			try
			{
				using (var stream = File.OpenRead(FilePath.Value))
				{
					var serializer = new XmlSerializer(typeof(PersistentStats));
					saved = (PersistentStats)serializer.Deserialize(stream);
				}

				return new Statistics(saved.Periods);
			}
			catch (Exception e)
			{
				Tracer.WriteError("Error has occurred during loading of persisted statistic", e);
			}

			return new Statistics();
		}

		public static void Save(Statistics statistics)
		{
			if (!Directory.Exists(DirPath.Value))
			{
				Directory.CreateDirectory(DirPath.Value);
			}
			
			using (var stream = new FileStream(FilePath.Value, FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(PersistentStats));
				serializer.Serialize(stream, new PersistentStats(statistics));
			}
		}

		public class PersistentStats
		{
			public PersistentStats()
			{
				
			}

			public PersistentStats(Statistics statistics)
			{
				Periods = statistics.Periods.ToArray();
			}

			public WorkPeriod[] Periods { get; set; }
		}

		private static readonly Lazy<string> DirPath = new Lazy<string>(() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sedentary"));
		private static readonly Lazy<string> FilePath = new Lazy<string>(() => Path.Combine(DirPath.Value, "Stats.xml"));

		private const string StatsPath = @"%LOCALAPPDATA%\Sedentary\Stats.xml";
	}
}