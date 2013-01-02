using System;

/// <summary>
/// Provides data for the error events that deal with message sending.
/// </summary>
namespace GsmComm.Server
{
	public class MessageSendErrorEventArgs : MessageSendEventArgs
	{
		private Exception exception;

		/// <summary>
		/// Gets the exception that caused the error.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.Server.MessageSendErrorEventArgs" />.
		/// </summary>
		/// <param name="message">The message that failed sending.</param>
		/// <param name="destination">The destination the message was attempted to send to.</param>
		/// <param name="exception">The exception that caused the error.</param>
		public MessageSendErrorEventArgs(string message, string destination, Exception exception) : base(message, destination)
		{
			this.exception = exception;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.Server.MessageSendErrorEventArgs" />.
		/// </summary>
		/// <param name="message">The message that failed sending.</param>
		/// <param name="destination">The destination the message was attempted to send to.</param>
		/// <param name="exception">The exception that caused the error.</param>
		/// <param name="userName">The name of the user from which the action started.</param>
		public MessageSendErrorEventArgs(string message, string destination, Exception exception, string userName) : base(message, destination, userName)
		{
			this.exception = exception;
		}
	}
}