using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sedentary.Framework
{
	public static class Tracer
	{
	    static Tracer()
	    {
	        Trace.AutoFlush = true;
	    }

		private static string _filter;

		public static void Filter<T>()
		{
			Filter(typeof(T).Name);
		}

		public static void Filter(string filter)
		{
			_filter = filter;
		}

		public static void Write<TSource>(string message, params object[] messageArgs)
		{
			Write(typeof(TSource), message, messageArgs);
		}

		public static void Write(string message, params object[] messageArgs)
		{
			Write(GetCallingMember().DeclaringType, message, messageArgs);		
		}

		private static void Write(Type sourceType, string message, object[] messageArgs)
		{
			Write(sourceType.Name, message, messageArgs);		
		}

		private static void Write(string source, string message, object[] messageArgs)
		{
			if (!string.IsNullOrEmpty(_filter) && !_filter.Equals(source, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			Trace.WriteLine(string.Format(@"{0:hh\:mm\:ss} {1}: {2}", DateTime.Now.TimeOfDay, source, string.Format(message, messageArgs)));
		}

	    public static void WriteMethod(params object[] args)
	    {
	        MethodBase method = GetCallingMember();
	        var className = method.DeclaringType.Name;

	        Trace.WriteLine(
	            string.Format(@"{0:hh\:mm\:ss} {1}.{2}({3})",
	                DateTime.Now.TimeOfDay,
	                className,
	                method.Name,
	                string.Join(", ", args.Select(a => a.ToString()))));
	    }

	    public static void WriteExpression<T>(Expression<Func<T>> expression)
		{
			Trace.WriteLine(string.Format("{0} = {1}", expression, expression.Compile().Invoke()));
		}

		public static void WritePropertyValue(object propertyValue, [CallerMemberName] string propertyName = null)
		{
			var className = GetCallingMember().DeclaringType.Name;
			Trace.WriteLine(string.Format(@"{0:hh\:mm\:ss} {1}: {2}={3}", DateTime.Now.TimeOfDay, className, propertyName, propertyValue));
		}

		public static void WriteObject(object obj)
		{
			Trace.WriteLine(obj.GetType().Name);
			Trace.Indent();

			foreach (var property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				Trace.WriteLine(string.Format("{0} = {1}", property.Name, property.GetValue(obj)));
			}

			Trace.Unindent();
		}

		private static MethodBase GetCallingMember()
		{
			var stack = new StackTrace();
			foreach (var frame in stack.GetFrames())
			{
				MethodBase method = frame.GetMethod();

				if (method.DeclaringType == typeof (Tracer))
				{
					continue;
				}

				return method;
			}

			throw new Exception("Cannot get calling member");
		}

		public static void WriteError(string msg, Exception ex)
		{
			Write(msg);
			Trace.Indent();
			Trace.TraceError(ex.ToString());
			Trace.Unindent();
		}
	}
}