using System;

/// <summary>
/// Provides data for the events that deal with message sending.
/// </summary>
namespace GsmComm.Server
{
	public class MessageSendEventArgs : EventArgs
	{
		private string message;

		private string destination;

		private string userName;

		/// <summary>
		/// Gets the destination the message is being or was sent to.
		/// </summary>
		public string Destination
		{
			get
			{
				return this.destination;
			}
		}

		/// <summary>
		/// Gets the message that is being sent or was sent.
		/// </summary>
		public string Message
		{
			get
			{
				return this.message;
			}
		}

		/// <summary>
		/// Gets the user name from which the action started.
		/// </summary>
		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.Server.MessageSendEventArgs" />.
		/// </summary>
		/// <param name="message">The message that is being sent or was sent.</param>
		/// <param name="destination">The destination the message is being or was sent to.</param>
		public MessageSendEventArgs(string message, string destination)
		{
			this.message = message;
			this.destination = destination;
			this.userName = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.Server.MessageSendEventArgs" />.
		/// </summary>
		/// <param name="message">The message that is being sent or was sent.</param>
		/// <param name="destination">The destination the message is being or was sent to.</param>
		/// <param name="userName">The name of the user from which the action started.</param>
		public MessageSendEventArgs(string message, string destination, string userName)
		{
			this.message = message;
			this.destination = destination;
			this.userName = userName;
		}
	}
}