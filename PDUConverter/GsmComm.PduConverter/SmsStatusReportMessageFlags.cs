using System;

/// <summary>
/// Represents the the first octet of an SMS-STATUS-REPORT PDU.
/// </summary>
namespace GsmComm.PduConverter
{
	public class SmsStatusReportMessageFlags : IncomingMessageFlags
	{
		private const byte TP_MTI_SMS_Status_Report = 2;

		private const byte TP_UDHI = 4;

		private const byte TP_MMS = 8;

		private const byte TP_SRQ = 16;

		private bool userDataHeaderPresent;

		private bool moreMessages;

		private bool qualifier;

		/// <summary>
		/// Parameter describing the message type.
		/// </summary>
		public override IncomingMessageType MessageType
		{
			get
			{
				return IncomingMessageType.SmsStatusReport;
			}
		}

		/// <summary>
		/// Parameter indicating whether or not there are more messages to send.
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
		/// Parameter indicating whether the previously submitted TPDU was an
		/// SMS-SUBMIT or an SMS-COMMAND.
		/// </summary>
		/// <remarks>
		/// <para>false = SMS-STATUS-REPORT is the result of a SMS-SUBMIT</para>
		/// <para>true = SMS-STATUS-REPORT is the result of a SMS-COMMAND</para>
		/// </remarks>
		public bool Qualifier
		{
			get
			{
				return this.qualifier;
			}
			set
			{
				this.qualifier = value;
			}
		}

		/// <summary>
		/// Parameter indicating that the TP-UD field contains a Header.
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
		public SmsStatusReportMessageFlags()
		{
			this.userDataHeaderPresent = false;
			this.moreMessages = false;
			this.qualifier = false;
		}

		/// <summary>
		/// Initializes a new instance of the IncomingMessageFlags class with a
		/// predefined data byte.
		/// </summary>
		public SmsStatusReportMessageFlags(byte flags)
		{
			this.FromByte(flags);
		}

		/// <summary>
		/// Fills the object with values from the data byte.
		/// </summary>
		/// <param name="b">The byte value.</param>
		protected override void FromByte(byte b)
		{
			IncomingMessageType incomingMessageType = IncomingMessageType.SmsStatusReport;
			if ((b & 2) > 0)
			{
				incomingMessageType = IncomingMessageType.SmsStatusReport;
			}
			if (incomingMessageType == IncomingMessageType.SmsStatusReport)
			{
				this.userDataHeaderPresent = (b & 4) > 0;
				this.moreMessages = (b & 8) == 0;
				this.qualifier = (b & 16) > 0;
				return;
			}
			else
			{
				throw new ArgumentException("Not an SMS-STATUS-REPORT message.");
			}
		}

		/// <summary>
		/// Returns the byte equivalent of this instance.
		/// </summary>
		/// <returns>The byte value.</returns>
		public override byte ToByte()
		{
			byte num = 0;
			num = (byte)(num | 2);
			if (this.userDataHeaderPresent)
			{
				num = (byte)(num | 4);
			}
			if (!this.moreMessages)
			{
				num = (byte)(num | 8);
			}
			if (this.qualifier)
			{
				num = (byte)(num | 16);
			}
			return num;
		}
	}
}