using System;

/// <summary>
/// Represents an SMS-DELIVER PDU, a received short message.
/// </summary>
namespace GsmComm.PduConverter
{
	public class SmsDeliverPdu : IncomingSmsPdu
	{
		private byte originatingAddressType;

		private string originatingAddress;

		private SmsTimestamp scTimestamp;

		/// <summary>
		/// Gets the message flags.
		/// </summary>
		/// <exception cref="T:System.ArgumentNullException">The property is being set
		/// and the value is null.</exception>
		public SmsDeliverMessageFlags MessageFlags
		{
			get
			{
				return (SmsDeliverMessageFlags)this.messageFlags;
			}
		}

		/// <summary>
		/// Gets or sets if there are more messages to send.
		/// </summary>
		public bool MoreMessages
		{
			get
			{
				return this.MessageFlags.MoreMessages;
			}
			set
			{
				this.MessageFlags.MoreMessages = value;
			}
		}

		/// <summary>
		/// Gets or sets the originating address.
		/// </summary>
		/// <remarks>
		/// <para>When setting the property, also the <see cref="P:GsmComm.PduConverter.SmsDeliverPdu.OriginatingAddressType" /> property
		/// will be set, attempting to autodetect the address type.</para>
		/// <para>When getting the property, the address may be extended with address-type
		/// specific prefixes or other chraracters.</para>
		/// </remarks>
		public string OriginatingAddress
		{
			get
			{
				return base.CreateAddressOfType(this.originatingAddress, this.originatingAddressType);
			}
			set
			{
				byte num = 0;
				string str = null;
				base.FindTypeOfAddress(value, out num, out str);
				this.originatingAddress = str;
				this.originatingAddressType = num;
			}
		}

		/// <summary>
		/// Gets or sets the type of the originating address.
		/// </summary>
		/// <remarks>
		/// <para>Represents the Type-of-Address octets for the originating address of the PDU.</para>
		/// </remarks>
		public byte OriginatingAddressType
		{
			get
			{
				return this.originatingAddressType;
			}
		}

		/// <summary>
		/// Gets or sets if a reply path exists.
		/// </summary>
		public bool ReplyPathExists
		{
			get
			{
				return this.MessageFlags.ReplyPathExists;
			}
			set
			{
				this.MessageFlags.ReplyPathExists = value;
			}
		}

		/// <summary>
		/// Gets or sets the timestamp the message was received by the SC.
		/// </summary>
		public SmsTimestamp SCTimestamp
		{
			get
			{
				return this.scTimestamp;
			}
			set
			{
				this.scTimestamp = value;
			}
		}

		/// <summary>
		/// Gets or sets if a status report was be requested.
		/// </summary>
		public bool StatusReportRequested
		{
			get
			{
				return this.MessageFlags.StatusReportRequested;
			}
			set
			{
				this.MessageFlags.StatusReportRequested = value;
			}
		}

		/// <summary>
		/// Gets or sets if a user data header is present.
		/// </summary>
		public override bool UserDataHeaderPresent
		{
			get
			{
				return this.MessageFlags.UserDataHeaderPresent;
			}
			set
			{
				this.MessageFlags.UserDataHeaderPresent = value;
			}
		}

		/// <summary>
		/// Initializes a new <see cref="T:GsmComm.PduConverter.SmsDeliverPdu" /> instance using default values.
		/// </summary>
		public SmsDeliverPdu()
		{
			this.messageFlags = new SmsDeliverMessageFlags();
			this.originatingAddress = string.Empty;
			this.originatingAddressType = 0;
			this.scTimestamp = SmsTimestamp.None;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsDeliverPdu" /> class
		/// using the specified PDU string.
		/// </summary>
		/// <param name="pdu">The PDU string to convert.</param>
		/// <param name="includesSmscData">Specifies if the string contains
		/// SMSC data octets at the beginning.</param>
		/// <param name="actualLength">Specifies the actual PDU length, that is the length in bytes without
		/// the SMSC header. Set to -1 if unknown.</param>
		/// <remarks>
		/// <para>This constructor assumes that the string contains an <b>SMS-DELIVER</b>
		/// PDU data stream as specified by GSM 07.05.</para>
		/// </remarks>
		public SmsDeliverPdu(string pdu, bool includesSmscData, int actualLength)
		{
			string str = null;
			byte num = 0;
			byte num1 = 0;
			byte[] numArray = null;
			if (pdu != string.Empty)
			{
				bool flag = actualLength >= 0;
				int num2 = actualLength;
				if (!flag || num2 > 0)
				{
					int num3 = 0;
					if (includesSmscData)
					{
						PduParts.DecodeSmscAddress(pdu, ref num3, out str, out num);
						base.SetSmscAddress(str, num);
					}
					int num4 = num3;
					num3 = num4 + 1;
					this.messageFlags = new SmsDeliverMessageFlags(BcdWorker.GetByte(pdu, num4));
					if (flag)
					{
						num2--;
						if (num2 <= 0)
						{
							base.ConstructLength = num3 * 2;
							return;
						}
					}
					PduParts.DecodeGeneralAddress(pdu, ref num3, out this.originatingAddress, out this.originatingAddressType);
					if (num3 * 2 < pdu.Length)
					{
						int num5 = num3;
						num3 = num5 + 1;
						base.ProtocolID = BcdWorker.GetByte(pdu, num5);
						int num6 = num3;
						num3 = num6 + 1;
						base.DataCodingScheme = BcdWorker.GetByte(pdu, num6);
						this.scTimestamp = new SmsTimestamp(pdu, ref num3);
						PduParts.DecodeUserData(pdu, ref num3, base.DataCodingScheme, out num1, out numArray);
						base.SetUserData(numArray, num1);
						base.ConstructLength = num3 * 2;
						return;
					}
					else
					{
						this.scTimestamp = SmsTimestamp.None;
						base.ProtocolID = 145;
						base.DataCodingScheme = 137;
						base.ConstructLength = num3 * 2;
						return;
					}
				}
				else
				{
					return;
				}
			}
			else
			{
				throw new ArgumentException("pdu must not be an empty string.");
			}
		}

		/// <summary>
		/// Returns the relevant timestamp for the message.
		/// </summary>
		/// <returns>An <see cref="T:GsmComm.PduConverter.SmsTimestamp" /> containing the SMSC timestamp,
		/// the time the message was received by the service center.</returns>
		public override SmsTimestamp GetTimestamp()
		{
			return this.scTimestamp;
		}

		/// <summary>
		/// Converts the value of this instance into a string.
		/// </summary>
		/// <param name="excludeSmscData">If true, excludes the SMSC header.</param>
		/// <returns>The encoded string.</returns>
		/// <remarks>Not implemented, always throws an <see cref="T:System.NotImplementedException" />.</remarks>
		public override string ToString(bool excludeSmscData)
		{
			throw new NotImplementedException("SmsDeliverPdu.ToString() not implemented.");
		}
	}
}