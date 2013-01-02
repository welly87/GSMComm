using System;

/// <summary>
/// Represents an SMS-STATUS-REPORT PDU, a status report message.
/// </summary>
namespace GsmComm.PduConverter
{
	public class SmsStatusReportPdu : IncomingSmsPdu
	{
		private byte messageReference;

		private byte recipientAddressType;

		private string recipientAddress;

		private SmsTimestamp scTimestamp;

		private SmsTimestamp dischargeTime;

		private MessageStatus status;

		private ParameterIndicator parameterIndicator;

		/// <summary>
		/// Gets or sets the parameter identifying the time associated with a
		/// particular TP-ST outcome.
		/// </summary>
		public SmsTimestamp DischargeTime
		{
			get
			{
				return this.dischargeTime;
			}
			set
			{
				this.dischargeTime = value;
			}
		}

		/// <summary>
		/// Gets the message flags.
		/// </summary>
		public SmsStatusReportMessageFlags MessageFlags
		{
			get
			{
				return (SmsStatusReportMessageFlags)this.messageFlags;
			}
		}

		/// <summary>
		/// Gets or sets the parameter identifying the previously submitted
		/// SMS-SUBMIT or an SMS-COMMAND.
		/// </summary>
		public byte MessageReference
		{
			get
			{
				return this.messageReference;
			}
			set
			{
				this.messageReference = value;
			}
		}

		/// <summary>
		/// Parameter indicating whether or not there are more messages to send.
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
		/// Gets or sets the parameter indicating the presence of any of the
		/// optional parameters which follow.
		/// </summary>
		public ParameterIndicator ParameterIndicator
		{
			get
			{
				return this.parameterIndicator;
			}
			set
			{
				this.parameterIndicator = value;
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
				return this.MessageFlags.Qualifier;
			}
			set
			{
				this.MessageFlags.Qualifier = value;
			}
		}

		/// <summary>
		/// Gets or sets the address of the recipient of the previously
		/// submitted mobile originated short message.
		/// </summary>
		/// <remarks>
		/// <para>When setting the property, also the <see cref="P:GsmComm.PduConverter.SmsStatusReportPdu.RecipientAddressType" /> property
		/// will be set, attempting to autodetect the address type.</para>
		/// <para>When getting the property, the address may be extended with address-type
		/// specific prefixes or other chraracters.</para>
		/// </remarks>
		public string RecipientAddress
		{
			get
			{
				return base.CreateAddressOfType(this.recipientAddress, this.recipientAddressType);
			}
			set
			{
				byte num = 0;
				string str = null;
				base.FindTypeOfAddress(value, out num, out str);
				this.recipientAddress = str;
				this.recipientAddressType = num;
			}
		}

		/// <summary>
		/// Gets the type of the recipient address.
		/// </summary>
		/// <remarks>
		/// <para>Represents the Type-of-Address octets for the recipient address of the PDU.</para>
		/// </remarks>
		public byte RecipientAddressType
		{
			get
			{
				return this.recipientAddressType;
			}
		}

		/// <summary>
		/// Gets or sets the parameter identifying time when the SC received
		/// the previously sent SMS-SUBMIT.
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
		/// Gets or sets the parameter identifying the status of the previously
		/// sent mobile originated short message.
		/// </summary>
		public MessageStatus Status
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
		/// Parameter indicating that the TP-UD field contains a Header.
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
		/// Initializes a new <see cref="T:GsmComm.PduConverter.SmsStatusReportPdu" /> instance using default values.
		/// </summary>
		public SmsStatusReportPdu()
		{
			this.messageFlags = new SmsStatusReportMessageFlags();
			this.messageReference = 0;
			this.RecipientAddress = string.Empty;
			this.scTimestamp = SmsTimestamp.None;
			this.dischargeTime = SmsTimestamp.None;
			this.status = 0;
			this.parameterIndicator = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsStatusReportPdu" /> class
		/// using the specified PDU string.
		/// </summary>
		/// <param name="pdu">The PDU string to convert.</param>
		/// <param name="includesSmscData">Specifies if the string contains
		/// SMSC data octets at the beginning.</param>
		/// <param name="actualLength">Specifies the actual PDU length, that is the length in bytes without
		/// the SMSC header. Set to -1 if unknown.</param>
		/// <remarks>
		/// <para>This constructor assumes that the string contains an <b>SMS-STATUS-REPORT</b>
		/// PDU data stream as specified
		/// by GSM 07.05.</para>
		/// </remarks>
		public SmsStatusReportPdu(string pdu, bool includesSmscData, int actualLength)
		{
			string str = null;
			byte num = 0;
			ParameterIndicator parameterIndicator;
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
					this.messageFlags = new SmsStatusReportMessageFlags(BcdWorker.GetByte(pdu, num4));
					if (flag)
					{
						num2--;
						if (num2 <= 0)
						{
							base.ConstructLength = num3 * 2;
							return;
						}
					}
					int num5 = num3;
					num3 = num5 + 1;
					this.messageReference = BcdWorker.GetByte(pdu, num5);
					PduParts.DecodeGeneralAddress(pdu, ref num3, out this.recipientAddress, out this.recipientAddressType);
					if (num3 * 2 < pdu.Length)
					{
						this.scTimestamp = new SmsTimestamp(pdu, ref num3);
						this.dischargeTime = new SmsTimestamp(pdu, ref num3);
						int num6 = num3;
						num3 = num6 + 1;
						this.status = BcdWorker.GetByte(pdu, num6);
						int num7 = BcdWorker.CountBytes(pdu);
						if (num3 < num7)
						{
							int num8 = num3;
							num3 = num8 + 1;
							this.parameterIndicator = new ParameterIndicator(BcdWorker.GetByte(pdu, num8));
							if (this.parameterIndicator.Extension)
							{
								do
								{
									int num9 = BcdWorker.CountBytes(pdu);
									if (num3 < num9)
									{
										int num10 = num3;
										num3 = num10 + 1;
										parameterIndicator = new ParameterIndicator(BcdWorker.GetByte(pdu, num10));
									}
									else
									{
										base.ConstructLength = num3 * 2;
										return;
									}
								}
								while (parameterIndicator.Extension);
							}
							if (this.parameterIndicator.TP_PID)
							{
								int num11 = num3;
								num3 = num11 + 1;
								base.ProtocolID = BcdWorker.GetByte(pdu, num11);
							}
							if (this.parameterIndicator.TP_DCS)
							{
								int num12 = num3;
								num3 = num12 + 1;
								base.DataCodingScheme = BcdWorker.GetByte(pdu, num12);
							}
							if (this.parameterIndicator.TP_UDL)
							{
								PduParts.DecodeUserData(pdu, ref num3, base.DataCodingScheme, out num1, out numArray);
								base.SetUserData(numArray, num1);
							}
							base.ConstructLength = num3 * 2;
							return;
						}
						else
						{
							base.ConstructLength = num3 * 2;
							return;
						}
					}
					else
					{
						base.ProtocolID = 145;
						base.DataCodingScheme = 137;
						this.SCTimestamp = SmsTimestamp.None;
						this.DischargeTime = SmsTimestamp.None;
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
		/// <returns>An <see cref="T:GsmComm.PduConverter.SmsTimestamp" /> containing the discharge time, the timestamp where
		/// the status in this report occurred.</returns>
		public override SmsTimestamp GetTimestamp()
		{
			return this.dischargeTime;
		}

		/// <summary>
		/// Sets the recipient address and type directly without attempting to
		/// autodetect the type.
		/// </summary>
		/// <param name="address">The recipient address</param>
		/// <param name="addressType">The address type</param>
		public void SetRecipientAddress(string address, byte addressType)
		{
			this.recipientAddress = address;
			this.recipientAddressType = addressType;
		}

		/// <summary>
		/// Converts the value of this instance into a string.
		/// </summary>
		/// <param name="excludeSmscData">If true, excludes the SMSC header.</param>
		/// <returns>The encoded string.</returns>
		/// <remarks>Not implemented, always throws an <see cref="T:System.NotImplementedException" />.</remarks>
		public override string ToString(bool excludeSmscData)
		{
			throw new NotImplementedException("SmsStatusReportPdu.ToString() not implemented.");
		}
	}
}