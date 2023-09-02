using QuickJs;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickJs
{
	public struct ErrorInfo
	{
		internal unsafe static bool TryCreate(JSContext* ctx, JSValue exception, out ErrorInfo errorInfo)
		{
            errorInfo = new ErrorInfo();

            if (NativeMethods.JS_IsError(ctx, exception)==0)
				return false;
            
            errorInfo.Name = exception.GetStringProperty(ctx, "name");
			errorInfo.Message = exception.GetStringProperty(ctx, "message");
			errorInfo.Stack = exception.GetStringProperty(ctx, "stack");
			return true;
		}

		/// <summary>
		/// Gets the error name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the error message.
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Get the stack trace.
		/// </summary>
		public string Stack { get; private set; }

	}
}
