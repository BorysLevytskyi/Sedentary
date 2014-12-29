using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Sedentary.Framework;

namespace Sedentary.Model.Persistence
{
	public static class StatsRepo
	{
		public static Statistics Get()
		{
			if (!File.Exists(FilePath.Value))
			{
				Tracer.Write("Not found previously saved statistics. New statistics created.");
				return new Statistics();
			}

			try
			{
				PersistentStats saved;

				using (var stream = File.OpenRead(FilePath.Value))
				{
					var serializer = new XmlSerializer(typeof(PersistentStats));
					saved = (PersistentStats)serializer.Deserialize(stream);
				}

				Tracer.Write("Statistics are loaded from file");
				return new Statistics(saved.Periods.Select(p => p.CreatePeriod()).ToList());
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
				Periods = statistics.Periods.Select(p => new PersistentWorkPeriod(p)).ToArray();
			}

			[XmlArray]
			public PersistentWorkPeriod[] Periods { get; set; }
		}

		[XmlType("Period")]
		public class PersistentWorkPeriod
		{
			public PersistentWorkPeriod()
			{
			}

			public PersistentWorkPeriod(WorkPeriod period)
			{
				State = period.State;
				StartTime = period.StartTime.ToString(TimeSpanPersistFormat);
				EndTime = period.EndTime.ToString(TimeSpanPersistFormat);
			}

			[XmlAttribute]
			public string StartTime { get; set; }

			[XmlAttribute]
			public string EndTime { get; set; }

			[XmlAttribute]
			public WorkState State { get; set; }

			public WorkPeriod CreatePeriod()
			{
				return new WorkPeriod(
					State,
					TimeSpan.Parse(StartTime),
					TimeSpan.Parse(EndTime));
			}
		}

		private const string TimeSpanPersistFormat = @"hh\:mm\:ss";

		private static readonly Lazy<string> DirPath = new Lazy<string>(() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sedentary"));
		private static readonly Lazy<string> FilePath = new Lazy<string>(() => Path.Combine(DirPath.Value, "Stats.xml"));
	}
}