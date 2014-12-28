using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace SittingTracker.Framework
{
	public static class Tracer
	{
		static Tracer()
		{
			SetupListener();
		}

		public static void TraceMe(string message, params object[] messageArgs)
		{
			var stack = new StackTrace();
			var className = stack.GetFrame(1).GetMethod().DeclaringType.Name;
			Trace.WriteLine(string.Format(@"{0:hh\:mm\:ss} {1}: {2}", DateTime.Now.TimeOfDay, className, string.Format(message, messageArgs)));
			Trace.Flush();
		}

		public static void Stop()
		{
			if (_listener != null)
			{
				_listener.Dispose();
			}
		}

		public static void TraceMyPropertyChanged(object propertyValue, [CallerMemberName] string propertyName = null)
		{
			var stack = new StackTrace();
			var className = stack.GetFrame(1).GetMethod().DeclaringType.Name;
			Trace.WriteLine(string.Format(@"{0:hh\:mm\:ss} {1}: {2}={3}", DateTime.Now.TimeOfDay, className, propertyName, propertyValue));
			Trace.Flush();
		}

		private static void SetupListener()
		{
			const string path = ".\\trace.txt";
			_listener = new TextWriterTraceListener(path);
			Trace.Listeners.Add(_listener);
		}

		private static TextWriterTraceListener _listener;
	}
}