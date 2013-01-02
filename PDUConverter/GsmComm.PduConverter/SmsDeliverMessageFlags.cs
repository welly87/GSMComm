using System;

/// <summary>
/// Represents the the first octet of an SMS-DELIVER PDU.
/// </summary>
namespace GsmComm.PduConverter
{
	public class SmsDeliverMessageFlags : IncomingMessageFlags
	{
		private const byte TP_MTI_SMS_Deliver = 0;

		private const byte TP_MMS = 4;

		private const byte TP_SRI = 32;

		private const byte TP_UDHI = 64;

		private const byte TP_RP = 128;

		private bool moreMessages;

		private bool statusReportRequested;

		private bool userDataHeaderPresent;

		private bool replyPathExists;

		/// <summary>
		/// Gets the type of the message.
		/// </summary>
		/// <remarks>Always returns <see cref="F:GsmComm.PduConverter.IncomingMessageType.SmsDeliver" />.</remarks>
		public override IncomingMessageType MessageType
		{
			get
			{
				return IncomingMessageType.SmsDeliver;
			}
		}

		/// <summary>
		/// Gets or sets if there are more messages to send.
		/// </summary>
		public bool MoreMessages
		{
			get
			{
				return this.moreMessages;
			}
			set
			{
				this.moreMessages = value;
			}
		}

		/// <summary>
		/// Gets or sets if a reply path exists.
		/// </summary>
		public bool ReplyPathExists
		{
			get
			{
				return this.replyPathExists;
			}
			set
			{
				this.replyPathExists = value;
			}
		}

		/// <summary>
		/// Gets or sets if a status report was be requested.
		/// </summary>
		public bool StatusReportRequested
		{
			get
			{
				return this.statusReportRequested;
			}
			set
			{
				this.statusReportRequested = value;
			}
		}

		/// <summary>
		/// Gets or sets if a user data header is present.
		/// </summary>
		public bool UserDataHeaderPresent
		{
			get
			{
				return this.userDataHeaderPresent;
			}
			set
			{
				this.userDataHeaderPresent = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsDeliverMessageFlags" /> class.
		/// </summary>
		public SmsDeliverMessageFlags()
		{
			this.moreMessages = false;
			this.statusReportRequested = false;
			this.userDataHeaderPresent = false;
			this.replyPathExists = false;
		}

		/// <summary>
		/// Initializes a new instance of the SmsDeliverMessageFlags class with a
		/// predefined data byte.
		/// </summary>
		/// <param name="flags">The message flags as a byte value.</param>
		public SmsDeliverMessageFlags(byte flags)
		{
			this.FromByte(flags);
		}

		/// <summary>
		/// Fills the object with values from the data byte.
		/// </summary>
		/// <param name="b">The byte value.</param>
		protected override void FromByte(byte b)
		{
			IncomingMessageType incomingMessageType = IncomingMessageType.SmsDeliver;
			if (0 > 0)
			{
				incomingMessageType = IncomingMessageType.SmsDeliver;
			}
			if (incomingMessageType == IncomingMessageType.SmsDeliver)
			{
				this.moreMessages = (b & 4) == 0;
				this.StatusReportRequested = (b & 32) > 0;
				this.userDataHeaderPresent = (b & 64) > 0;
				this.replyPathExists = (b & 128) > 0;
				return;
			}
			else
			{
				throw new ArgumentException("Not an SMS-DELIVER message.");
			}
		}

		/// <summary>
		/// Returns the byte equivalent of this instance.
		/// </summary>
		/// <returns>The byte value.</returns>
		public override byte ToByte()
		{
			byte num = 0;
			num = (byte)num;
			if (!this.moreMessages)
			{
				num = (byte)(num | 4);
			}
			if (this.StatusReportRequested)
			{
				num = (byte)(num | 32);
			}
			if (this.userDataHeaderPresent)
			{
				num = (byte)(num | 64);
			}
			if (this.replyPathExists)
			{
				num = (byte)(num | 128);
			}
			return num;
		}
	}
}