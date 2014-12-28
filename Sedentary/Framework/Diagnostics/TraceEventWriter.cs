using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sedentary.Framework.Diagnostics
{
	public class TraceEventWriter : TraceListener
	{
		private readonly List<TraceEvent> _events = new List<TraceEvent>();

		public event Action Update;

		protected virtual void OnUpdate()
		{
			Action handler = Update;
			if (handler != null) handler();
		}

		public override void Write(string message)
		{
			WriteEvent(message);
		}

		public override void WriteLine(string message)
		{
			WriteEvent(message);
		}

		public List<TraceEvent> Events
		{
			get
			{
				lock (_sync)
				{
					return _events;
				}
			}
		}

		private void WriteEvent(string msg)
		{
			int hash = msg.GetHashCode();
			TraceEvent evt = Enumerable.Reverse(Events).FirstOrDefault(e => e.HashCode == hash);
			
			if (evt != null)
			{
				evt.Count++;
				return;
			}

			lock (_sync)
			{
				_events.Add(new TraceEvent(msg));
			}

			OnUpdate();
		}

		private readonly object _sync = new object();
	}
}