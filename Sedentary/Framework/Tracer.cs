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
		public static void Write(string message, params object[] messageArgs)
		{
			var stack = new StackTrace();
			var className = GetCallingMember().DeclaringType.Name;
			Trace.WriteLine(string.Format(@"{0:hh\:mm\:ss} {1}: {2}", DateTime.Now.TimeOfDay, className, string.Format(message, messageArgs)));
			Trace.Flush();
		}

		public static void WriteMethod(params object[] args)
		{
			var stack = new StackTrace();
			MethodBase method = GetCallingMember();
			var className = method.DeclaringType.Name;
			
			Trace.WriteLine(
				string.Format(@"{0:hh\:mm\:ss} {1}.{2}({3})", 
				DateTime.Now.TimeOfDay, 
				className, 
				method.Name,
				string.Join(", ", args.Select(a => a.ToString()))));

			Trace.Flush();
		}

		public static void WriteExpression<T>(Expression<Func<T>> expression)
		{
			Trace.WriteLine(string.Format("{0} = {1}", expression, expression.Compile().Invoke()));
		}

		public static void WritePropertyValue(object propertyValue, [CallerMemberName] string propertyName = null)
		{
			var className = GetCallingMember().DeclaringType.Name;
			Trace.WriteLine(string.Format(@"{0:hh\:mm\:ss} {1}: {2}={3}", DateTime.Now.TimeOfDay, className, propertyName, propertyValue));
			Trace.Flush();
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