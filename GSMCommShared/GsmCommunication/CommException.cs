using System;
using System.Runtime.Serialization;

/// <summary>
/// General exception that gets thrown upon a communication error with the device.
/// </summary>
namespace GsmComm.GsmCommunication
{
	[Serializable]
	public class CommException : ApplicationException
	{
		private string commTrace;

		/// <summary>
		/// Gets the communication trace associated with the exception.
		/// </summary>
		public string CommTrace
		{
			get
			{
				return this.commTrace;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.CommException" /> class.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		public CommException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.CommException" /> class with a specified error
		/// message and a communication trace.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		/// <param name="commTrace">The communication that occurred right before the error.</param>
		public CommException(string message, string commTrace) : base(message)
		{
			this.commTrace = commTrace;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.CommException" /> class with a specified error
		/// message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public CommException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.CommException" /> class with a specified error
		/// message, a communication trace and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		/// <param name="commTrace">The communication that occurred right before the error.</param> 
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public CommException(string message, string commTrace, Exception innerException) : base(message, innerException)
		{
			this.commTrace = commTrace;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.CommException" /> class with serialized data. 
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected CommException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}