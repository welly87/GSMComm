using GsmComm.PduConverter;
using System;

/// <summary>
/// Provides data for the error events that deal with message sending.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class MessageErrorEventArgs : MessageEventArgs
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
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.MessageErrorEventArgs" />.
		/// </summary>
		/// <param name="pdu">The message that failed sending.</param>
		/// <param name="exception">The exception that caused the error.</param>
		public MessageErrorEventArgs(OutgoingSmsPdu pdu, Exception exception) : base(pdu)
		{
			this.exception = exception;
		}
	}
}