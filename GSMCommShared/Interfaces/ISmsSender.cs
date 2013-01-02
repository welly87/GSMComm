using System;

/// <summary>
/// Defines the interface for an SMS Sender.
/// </summary>
namespace GsmComm.Interfaces
{
	public interface ISmsSender
	{
		/// <summary>
		/// Sends an SMS message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <param name="destination">The destination (phone number) to which the message should be sent.</param>
		void SendMessage(string message, string destination);

		/// <summary>
		/// Sends an SMS message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <param name="destination">The destination (phone number) to which the message should be sent.</param>
		/// <param name="unicode">Specifies if the message should be sent as Unicode.</param>
		void SendMessage(string message, string destination, bool unicode);
	}
}