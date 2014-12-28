using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Documents;

namespace Sedentary.Framework
{
	public class ObservableTraceListener : TraceListener
	{
		private readonly TraceListener _listener;

		public event Action Written;

		protected virtual void OnWritten()
		{
			Action handler = Written;
			if (handler != null) handler();
		}

		public ObservableTraceListener(TraceListener listener)
		{
			_listener = listener;
		}

		public override void Write(string message)
		{
			_listener.Write(message);
			OnWritten();
		}

		public override void WriteLine(string message)
		{
			_listener.WriteLine(message);
			OnWritten();
		}

		public override string Name
		{
			get { return _listener.Name; }
			set { _listener.Name = value; }
		}

		public override void Close()
		{
			_listener.Close();
		}

		protected override void Dispose(bool disposing)
		{
			_listener.Dispose();
		}
	}

	public class TraceEvent
	{
		public TraceEvent(string message)
		{
			Message = message;
			HashCode = message.GetHashCode();
			Count = 1;
		}

		public int Count { get; set; }

		public int HashCode { get; private set; }

		public string Message { get; private set; }

		public override string ToString()
		{
			return Count > 1 ? string.Format("({0}) {1}", Count, Message) : Message;
		}
	}
}
