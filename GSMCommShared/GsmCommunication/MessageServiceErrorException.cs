using System;
using System.Runtime.Serialization;

/// <summary>
/// The exception that gets thrown when there is an error with the message service.
/// </summary>
namespace GsmComm.GsmCommunication
{
	[Serializable]
	public class MessageServiceErrorException : CommException
	{
		private int errorCode;

		/// <summary>
		/// Gets the error code reported by the network.
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.MessageServiceErrorException" /> class with a specified error
		/// message, an error code and a communication trace.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		/// <param name="errorCode">The error code reported by the network.</param>
		/// <param name="commTrace">The communication that occurred right before the error.</param> 
		/// <remarks>This exception gets thrown when an action can not be executed due to a message service error.</remarks>
		public MessageServiceErrorException(string message, int errorCode, string commTrace) : base(message, commTrace)
		{
			this.errorCode = errorCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.MessageServiceErrorException" /> class with a specified error
		/// message, an error code, a communication trace and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		/// <param name="errorCode">The error code reported by the network.</param>
		/// <param name="commTrace">The communication that occurred right before the error.</param> 
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		/// <remarks>This exception gets thrown when an action can not be executed due to a message service error.</remarks>
		public MessageServiceErrorException(string message, int errorCode, string commTrace, Exception innerException) : base(message, commTrace, innerException)
		{
			this.errorCode = errorCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.MessageServiceErrorException" /> class with serialized data. 
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected MessageServiceErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}