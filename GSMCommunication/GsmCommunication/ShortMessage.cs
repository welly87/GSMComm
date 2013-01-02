using System;
using System.Xml.Serialization;

/// <summary>
/// Represents a short message in undecoded PDU format.
/// </summary>
namespace GsmComm.GsmCommunication
{
	[Serializable]
	[XmlInclude(typeof(ShortMessageFromPhone))]
	public class ShortMessage : IMessageIndicationObject
	{
		private string alpha;

		private int length;

		private string data;

		/// <summary>
		/// The alphabet in which the message is encoded.
		/// </summary>
		[XmlAttribute]
		public string Alpha
		{
			get
			{
				return this.alpha;
			}
			set
			{
				this.alpha = value;
			}
		}

		/// <summary>
		/// The actual message.
		/// </summary>
		[XmlElement]
		public string Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		/// <summary>
		/// The length of the message. In PDU format, this is the actual length without the SMSC header.
		/// </summary>
		[XmlAttribute]
		public int Length
		{
			get
			{
				return this.length;
			}
			set
			{
				this.length = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public ShortMessage()
		{
			this.alpha = string.Empty;
			this.length = 0;
			this.data = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.GsmCommunication.ShortMessage" /> class.
		/// </summary>
		/// <param name="alpha">The alphabet in which the message is encoded.</param>
		/// <param name="length">The length of the data.</param>
		/// <param name="data">The message.</param>
		public ShortMessage(string alpha, int length, string data)
		{
			this.Alpha = alpha;
			this.Length = length;
			this.Data = data;
		}
	}
}