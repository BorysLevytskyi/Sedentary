namespace Sedentary.Framework.Diagnostics
{
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