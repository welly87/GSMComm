using GsmComm.PduConverter;
using System;

/// <summary>
/// Provides data for the events that deal with message sending.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class MessageEventArgs : EventArgs
	{
		private OutgoingSmsPdu pdu;

		/// <summary>
		/// The message that was dealt with.
		/// </summary>
		public OutgoingSmsPdu Pdu
		{
			get
			{
				return this.pdu;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.MessageEventArgs" />.
		/// </summary>
		/// <param name="pdu">The message that was dealt with.</param>
		public MessageEventArgs(OutgoingSmsPdu pdu)
		{
			this.pdu = pdu;
		}
	}
}