using System;

/// <summary>
/// Represents the the first octet of an SMS-SUBMIT-PDU.
/// </summary>
namespace GsmComm.PduConverter
{
	public class SmsSubmitMessageFlags : OutgoingMessageFlags
	{
		private const byte TP_MTI_SMS_Submit = 1;

		private const byte TP_RD = 4;

		private const byte TP_VPF_Enhanced = 8;

		private const byte TP_VPF_Relative = 16;

		private const byte TP_VPF_Absolute = 24;

		private const byte TP_SRR = 32;

		private const byte TP_UDHI = 64;

		private const byte TP_RP = 128;

		private bool rejectDuplicates;

		private ValidityPeriodFormat validityPeriodFormat;

		private bool requestStatusReport;

		private bool userDataHeaderPresent;

		private bool replyPathExists;

		/// <summary>
		/// Gets the message type, always returns <see cref="F:GsmComm.PduConverter.OutgoingMessageType.SmsSubmit" />.
		/// </summary>
		public override OutgoingMessageType MessageType
		{
			get
			{
				return OutgoingMessageType.SmsSubmit;
			}
		}

		/// <summary>
		/// Gets or sets if the SC should reject duplicate messages.
		/// </summary>
		public bool RejectDuplicates
		{
			get
			{
				return this.rejectDuplicates;
			}
			set
			{
				this.rejectDuplicates = value;
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
		/// Gets or sets if s status report should be requested.
		/// </summary>
		public bool RequestStatusReport
		{
			get
			{
				return this.requestStatusReport;
			}
			set
			{
				this.requestStatusReport = value;
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
		/// Gets or sets the validity period format.
		/// </summary>
		public ValidityPeriodFormat ValidityPeriodFormat
		{
			get
			{
				return this.validityPeriodFormat;
			}
			set
			{
				this.validityPeriodFormat = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsSubmitMessageFlags" /> class.
		/// </summary>
		/// <remarks>
		/// Default settings are:
		/// <list type="bullet">
		/// <item><description>
		/// ReplyPathExists = false
		/// </description></item>
		/// <item><description>
		/// UserDataHeaderPresent = false
		/// </description></item>
		/// <item><description>
		/// RequestStatusReport = false
		/// </description></item>
		/// <item><description>
		/// ValidityPeriodFormat = ValidityPeriodFormat.Unspecified
		/// </description></item>
		/// <item><description>
		/// RejectDuplicates = false
		/// </description></item>
		/// <item><description>
		/// MessageType = OutgoingMessageType.SmsSubmit
		/// </description></item>
		/// </list>
		/// </remarks>
		public SmsSubmitMessageFlags()
		{
			this.rejectDuplicates = false;
			this.validityPeriodFormat = ValidityPeriodFormat.Unspecified;
			this.requestStatusReport = false;
			this.userDataHeaderPresent = false;
			this.replyPathExists = false;
		}

		/// <summary>
		/// Initializes a new instance of the MessageFlags class with a
		/// predefined data byte.
		/// </summary>
		public SmsSubmitMessageFlags(byte flags)
		{
			this.FromByte(flags);
		}

		/// <summary>
		/// Fills the object with values from the data byte.
		/// </summary>
		/// <param name="b">The byte value.</param>
		protected override void FromByte(byte b)
		{
			OutgoingMessageType outgoingMessageType = OutgoingMessageType.SmsSubmit;
			if ((b & 1) > 0)
			{
				outgoingMessageType = OutgoingMessageType.SmsSubmit;
			}
			if (outgoingMessageType == OutgoingMessageType.SmsSubmit)
			{
				this.rejectDuplicates = (b & 4) > 0;
				this.validityPeriodFormat = ValidityPeriodFormat.Unspecified;
				if ((b & 24) > 0)
				{
					this.validityPeriodFormat = ValidityPeriodFormat.Absolute;
				}
				if ((b & 16) > 0)
				{
					this.validityPeriodFormat = ValidityPeriodFormat.Relative;
				}
				if ((b & 8) > 0)
				{
					this.validityPeriodFormat = ValidityPeriodFormat.Enhanced;
				}
				this.requestStatusReport = (b & 32) > 0;
				this.userDataHeaderPresent = (b & 64) > 0;
				this.replyPathExists = (b & 128) > 0;
				return;
			}
			else
			{
				throw new ArgumentException("Not an SMS-SUBMIT message.");
			}
		}

		/// <summary>
		/// Returns the byte equivalent of this instance.
		/// </summary>
		/// <returns>The byte value.</returns>
		public override byte ToByte()
		{
			byte num = 0;
			num = (byte)(num | 1);
			if (this.rejectDuplicates)
			{
				num = (byte)(num | 4);
			}
			ValidityPeriodFormat validityPeriodFormat = this.validityPeriodFormat;
			switch (validityPeriodFormat)
			{
				case ValidityPeriodFormat.Relative:
				{
					num = (byte)(num | 16);
					break;
				}
				case ValidityPeriodFormat.Absolute:
				{
					num = (byte)(num | 24);
					break;
				}
				case ValidityPeriodFormat.Enhanced:
				{
					num = (byte)(num | 8);
					break;
				}
			}
			if (this.requestStatusReport)
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

		/// <summary>
		/// Returns the string equivalent of this instance.
		/// </summary>
		public override string ToString()
		{
			byte num = this.ToByte();
			return num.ToString();
		}
	}
}