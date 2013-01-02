using GsmComm.PduConverter;
using System;

/// <summary>
/// Represents a short message from the phone in its decoded state.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class DecodedShortMessage
	{
		private int index;

		private SmsPdu data;

		private PhoneMessageStatus status;

		private string storage;

		/// <summary>
		/// Gets the decoded message.
		/// </summary>
		public SmsPdu Data
		{
			get
			{
				return this.data;
			}
		}

		/// <summary>
		/// Gets the index where the message is saved in the device in the <see cref="P:GsmComm.GsmCommunication.DecodedShortMessage.Storage" />.
		/// </summary>
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		/// <summary>
		/// Gets the parsed message status.
		/// </summary>
		public PhoneMessageStatus Status
		{
			get
			{
				return this.status;
			}
		}

		/// <summary>
		/// Gets the phone storage the message was read from.
		/// </summary>
		public string Storage
		{
			get
			{
				return this.storage;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.ShortMessageFromPhone" />class.
		/// </summary>
		/// <param name="index">The index where the message is saved in the device in the <see cref="F:GsmComm.GsmCommunication.DecodedShortMessage.storage" />.</param>
		/// <param name="data">The decoded message.</param>
		/// <param name="status">The parsed message status.</param>
		/// <param name="storage">The phone storage the message was read from.</param>
		public DecodedShortMessage(int index, SmsPdu data, PhoneMessageStatus status, string storage)
		{
			this.index = index;
			this.data = data;
			this.status = status;
			this.storage = storage;
		}
	}
}