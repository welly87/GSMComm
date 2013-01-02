using System;
using System.Xml.Serialization;

/// <summary>
/// Represents a short message read from the phone in undecoded PDU format.
/// </summary>
namespace GsmComm.GsmCommunication
{
	[Serializable]
	public class ShortMessageFromPhone : ShortMessage
	{
		private int index;

		private int status;

		/// <summary>
		/// The index of the message.
		/// </summary>
		[XmlAttribute]
		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		/// <summary>
		/// The message status (e.g. read, unread, etc.)
		/// </summary>
		[XmlAttribute]
		public int Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public ShortMessageFromPhone()
		{
			this.index = 0;
			this.status = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.ShortMessageFromPhone" /> class.
		/// </summary>
		/// <param name="index">The index where the message is saved in the device in the currently active storage.</param>
		/// <param name="status">The message status (e.g. read or unread)</param>
		/// <param name="alpha">The alphabet in which the message is encoded.</param>
		/// <param name="length">The length of the data.</param>
		/// <param name="data">The actual message.</param>
		/// <remarks>The object contains all data returned by the phone.
		/// </remarks>
		public ShortMessageFromPhone(int index, int status, string alpha, int length, string data) : base(alpha, length, data)
		{
			this.Index = index;
			this.Status = status;
		}
	}
}