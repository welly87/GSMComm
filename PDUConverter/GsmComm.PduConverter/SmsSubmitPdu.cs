using System;
using System.Text;

/// <summary>
/// Represents an SMS-SUBMIT PDU, an outgoing short message.
/// </summary>
namespace GsmComm.PduConverter
{
	public class SmsSubmitPdu : OutgoingSmsPdu
	{
		private byte destTOA;

		private string destAddress;

		private ValidityPeriod validityPeriod;

		/// <summary>
		/// Gets or sets the destination address.
		/// </summary>
		/// <remarks>
		/// <para>When setting the property also the <see cref="P:GsmComm.PduConverter.SmsSubmitPdu.DestinationAddressType" />
		/// property will be set, attempting to autodetect the address type.</para>
		/// <para>When getting the property: The address may be extended with address-type
		/// specific prefixes or other chraracters.</para>
		/// </remarks>
		public string DestinationAddress
		{
			get
			{
				return base.CreateAddressOfType(this.destAddress, this.destTOA);
			}
			set
			{
				byte num = 0;
				string str = null;
				base.FindTypeOfAddress(value, out num, out str);
				this.destAddress = str;
				this.destTOA = num;
			}
		}

		/// <summary>
		/// Gets the type of the destination address.
		/// </summary>
		/// <remarks>
		/// <para>Represents the Type-of-Address octets for the destination address of the PDU.</para>
		/// </remarks>
		public byte DestinationAddressType
		{
			get
			{
				return this.destTOA;
			}
		}

		/// <summary>
		/// Gets the message flags.
		/// </summary>
		public SmsSubmitMessageFlags MessageFlags
		{
			get
			{
				return (SmsSubmitMessageFlags)this.messageFlags;
			}
		}

		/// <summary>
		/// Gets or sets if the SC should reject duplicate messages.
		/// </summary>
		public bool RejectDuplicates
		{
			get
			{
				return this.MessageFlags.RejectDuplicates;
			}
			set
			{
				this.MessageFlags.RejectDuplicates = value;
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
		/// Gets or sets if s status report should be requested.
		/// </summary>
		public bool RequestStatusReport
		{
			get
			{
				return this.MessageFlags.RequestStatusReport;
			}
			set
			{
				this.MessageFlags.RequestStatusReport = value;
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
		/// Gets or sets the validity period.
		/// </summary>
		/// <remarks>
		/// <para>Represents the TP-Validity-Period octet of the PDU.</para>
		/// <para>The validity period specifies the time when SM expires. If SM is't delivered
		/// before that moment, it is discarded by SC. Validity-Period can be in
		/// three different formats: Relative, Enhanced and Absolute.</para>
		/// </remarks>
		public ValidityPeriod ValidityPeriod
		{
			get
			{
				return this.validityPeriod;
			}
			set
			{
				if (value as RelativeValidityPeriod == null)
				{
					if (value != null)
					{
						throw new ArgumentException(string.Concat("Unknown or unsupported validity period format: ", value.GetType().ToString()));
					}
					else
					{
						this.MessageFlags.ValidityPeriodFormat = ValidityPeriodFormat.Unspecified;
					}
				}
				else
				{
					this.MessageFlags.ValidityPeriodFormat = ValidityPeriodFormat.Relative;
				}
				this.validityPeriod = value;
			}
		}

		/// <summary>
		/// Initializes a new <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> instance using default values.
		/// </summary>
		public SmsSubmitPdu()
		{
			this.messageFlags = new SmsSubmitMessageFlags();
			this.DestinationAddress = string.Empty;
			this.ValidityPeriod = new RelativeValidityPeriod(167);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> class
		/// using the specified text and destination address.
		/// </summary>
		/// <param name="userDataText">The message text, not exceeding 160 characters.</param>
		/// <param name="destinationAddress">The message's destination address.</param>
		public SmsSubmitPdu(string userDataText, string destinationAddress) : this()
		{
			base.Encode7BitText(userDataText);
			this.DestinationAddress = destinationAddress;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> class using the specified text,
		/// destination address and SMSC address.
		/// </summary>
		/// <param name="userDataText">The message text, not exceeding 160 characters.</param>
		/// <param name="destinationAddress">The message's destination address.</param>
		/// <param name="smscAddress">The service center (SMSC) address. Can be an empty string.</param>
		public SmsSubmitPdu(string userDataText, string destinationAddress, string smscAddress) : this()
		{
			base.Encode7BitText(userDataText);
			this.DestinationAddress = destinationAddress;
			base.SmscAddress = smscAddress;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> class using the specified text,
		/// destination address and data coding scheme.
		/// </summary>
		/// <param name="userDataText">The message text.</param>
		/// <param name="destinationAddress">The message's destination address.</param>
		/// <param name="dataCodingScheme">Specifies how the userDataText should be encoded.</param>
		/// <remarks><para>The maximum length of the userDataText parameter depends on the alphabet specified with
		/// the dataCodingScheme parameter.</para><para>Common values for the dataCodingScheme are
		/// <see cref="F:GsmComm.PduConverter.DataCodingScheme.NoClass_7Bit" /> for GSM default alphabet and
		/// <see cref="F:GsmComm.PduConverter.DataCodingScheme.NoClass_16Bit" /> for UCS2 alphabet (Unicode).</para></remarks>
		public SmsSubmitPdu(string userDataText, string destinationAddress, byte dataCodingScheme) : this()
		{
			base.DataCodingScheme = dataCodingScheme;
			base.UserDataText = userDataText;
			this.DestinationAddress = destinationAddress;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> class using the specified text,
		/// destination address, SMSC address and data coding scheme.
		/// </summary>
		/// <param name="userDataText">The message text.</param>
		/// <param name="destinationAddress">The message's destination address.</param>
		/// <param name="smscAddress">The service center (SMSC) address. Can be an empty string.</param>
		/// <param name="dataCodingScheme">Specifies how the userDataText should be encoded.</param>
		/// <remarks><para>The maximum length of the userDataText parameter depends on the alphabet specified with
		/// the dataCodingScheme parameter.</para><para>Common values for the dataCodingScheme are
		/// <see cref="F:GsmComm.PduConverter.DataCodingScheme.NoClass_7Bit" /> for GSM default alphabet and
		/// <see cref="F:GsmComm.PduConverter.DataCodingScheme.NoClass_16Bit" /> for UCS2 alphabet (Unicode).</para></remarks>
		public SmsSubmitPdu(string userDataText, string destinationAddress, string smscAddress, byte dataCodingScheme) : this()
		{
			base.DataCodingScheme = dataCodingScheme;
			base.UserDataText = userDataText;
			this.DestinationAddress = destinationAddress;
			base.SmscAddress = smscAddress;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> class
		/// using the specified PDU string.
		/// </summary>
		/// <param name="pdu">The PDU string to convert.</param>
		/// <param name="includesSmscData">Specifies if the string contains
		/// SMSC data octets at the beginning.</param>
		/// <param name="actualLength">Specifies the actual PDU length, that is the length in bytes without
		/// the SMSC header. Set to -1 if unknown.</param>
		/// <remarks>
		/// <para>This constructor assumes that the string contains an <b>SMS-SUBMIT</b>
		/// PDU data stream as specified
		/// by GSM 07.05.</para>
		/// <para>AbsuluteValidityPeriod and EnhancedValidityPeriod are not
		/// supported and will generate a <see cref="T:System.NotSupportedException" />
		/// when encountered.
		/// </para>
		/// </remarks>
		/// <exception cref="T:System.NotSupportedException">The string contains a
		/// validity and the validity period format is not relative validity.
		/// </exception>
		public SmsSubmitPdu(string pdu, bool includesSmscData, int actualLength)
		{
			byte num = 0;
			byte[] numArray = null;
			int num1;
			if (pdu != string.Empty)
			{
				bool flag = actualLength >= 0;
				int num2 = actualLength;
				if (!flag || num2 > 0)
				{
					int num3 = 0;
					if (!includesSmscData)
					{
						base.SmscAddress = string.Empty;
					}
					else
					{
						int num4 = num3;
						num3 = num4 + 1;
						byte num5 = BcdWorker.GetByte(pdu, num4);
						if (num5 <= 0)
						{
							base.SmscAddress = string.Empty;
						}
						else
						{
							int num6 = num3;
							num3 = num6 + 1;
							byte num7 = BcdWorker.GetByte(pdu, num6);
							int num8 = num5 - 1;
							string bytesString = BcdWorker.GetBytesString(pdu, num3, num8);
							num3 = num3 + num8;
							string str = BcdWorker.DecodeSemiOctets(bytesString);
							if (str.EndsWith("F") || str.EndsWith("f"))
							{
								str = str.Substring(0, str.Length - 1);
							}
							base.SetSmscAddress(str, num7);
						}
					}
					int num9 = num3;
					num3 = num9 + 1;
					this.messageFlags = new SmsSubmitMessageFlags(BcdWorker.GetByte(pdu, num9));
					if (flag)
					{
						num2--;
						if (num2 <= 0)
						{
							base.ConstructLength = num3 * 2;
							return;
						}
					}
					int num10 = num3;
					num3 = num10 + 1;
					base.MessageReference = BcdWorker.GetByte(pdu, num10);
					int num11 = num3;
					num3 = num11 + 1;
					byte num12 = BcdWorker.GetByte(pdu, num11);
					int num13 = num3;
					num3 = num13 + 1;
					byte num14 = BcdWorker.GetByte(pdu, num13);
					if (num12 <= 0)
					{
						this.DestinationAddress = string.Empty;
					}
					else
					{
						if (num12 % 2 != 0)
						{
							num1 = num12 + 1;
						}
						else
						{
							num1 = (int)num12;
						}
						int num15 = num1 / 2;
						string bytesString1 = BcdWorker.GetBytesString(pdu, num3, num15);
						num3 = num3 + num15;
						string str1 = BcdWorker.DecodeSemiOctets(bytesString1).Substring(0, num12);
						this.SetDestinationAddress(str1, num14);
					}
					int num16 = num3;
					num3 = num16 + 1;
					base.ProtocolID = BcdWorker.GetByte(pdu, num16);
					int num17 = num3;
					num3 = num17 + 1;
					base.DataCodingScheme = BcdWorker.GetByte(pdu, num17);
					ValidityPeriodFormat validityPeriodFormat = this.MessageFlags.ValidityPeriodFormat;
					if (validityPeriodFormat == ValidityPeriodFormat.Unspecified)
					{
						this.ValidityPeriod = null;
					}
					else if (validityPeriodFormat == ValidityPeriodFormat.Relative)
					{
						int num18 = num3;
						num3 = num18 + 1;
						this.ValidityPeriod = new RelativeValidityPeriod(BcdWorker.GetByte(pdu, num18));
					}
					else if (validityPeriodFormat == ValidityPeriodFormat.Absolute)
					{
						throw new NotSupportedException("Absolute validity period format not supported.");
					}
					else if (validityPeriodFormat == ValidityPeriodFormat.Enhanced)
					{
						throw new NotSupportedException("Enhanced validity period format not supported.");
					}
					else
					{
						throw new NotSupportedException(string.Concat("Validity period format \"", (object)this.MessageFlags.ValidityPeriodFormat.ToString(), "\" not supported."));
					}
					PduParts.DecodeUserData(pdu, ref num3, base.DataCodingScheme, out num, out numArray);
					base.SetUserData(numArray, num);
					base.ConstructLength = num3 * 2;
					return;
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

		private string EncodeDestinationAddress(string address, byte addressType)
		{
			if (address.Length != 0)
			{
				byte length = (byte)address.Length;
				string str = BcdWorker.EncodeSemiOctets(address);
				return string.Concat(Calc.IntToHex(length), Calc.IntToHex(addressType), str);
			}
			else
			{
				return string.Concat(Calc.IntToHex(0), Calc.IntToHex(addressType));
			}
		}

		private string EncodeSmscAddress(string address, byte addressType)
		{
			if (address.Length != 0)
			{
				string str = BcdWorker.EncodeSemiOctets(address);
				byte length = (byte)(str.Length / 2 + 1);
				return string.Concat(Calc.IntToHex(length), Calc.IntToHex(addressType), str);
			}
			else
			{
				return Calc.IntToHex(0);
			}
		}

		/// <summary>
		/// Returns the relevant timestamp for the message.
		/// </summary>
		/// <returns>Always <see cref="F:GsmComm.PduConverter.SmsTimestamp.None" />. An <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> does not have
		/// a timestamp.</returns>
		public override SmsTimestamp GetTimestamp()
		{
			return SmsTimestamp.None;
		}

		/// <summary>
		/// Sets the destination address and type directly without attempting to
		/// autodetect the type.
		/// </summary>
		/// <param name="address">The destination address</param>
		/// <param name="addressType">The address type</param>
		public void SetDestinationAddress(string address, byte addressType)
		{
			this.destAddress = address;
			this.destTOA = addressType;
		}

		/// <summary>
		/// Converts the value of this instance into a string.
		/// </summary>
		/// <param name="excludeSmscData">If true, excludes the SMSC header.</param>
		/// <returns>The encoded string.</returns>
		public override string ToString(bool excludeSmscData)
		{
			string str = null;
			byte num = 0;
			string empty = string.Empty;
			if (this.MessageFlags.ValidityPeriodFormat != ValidityPeriodFormat.Relative)
			{
				if (this.MessageFlags.ValidityPeriodFormat != ValidityPeriodFormat.Unspecified)
				{
					throw new NotSupportedException(string.Concat("The specified validity period format \"", this.MessageFlags.ValidityPeriodFormat.ToString(), "\" is not supported."));
				}
			}
			else
			{
				empty = Calc.IntToHex((RelativeValidityPeriod)this.ValidityPeriod);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!excludeSmscData)
			{
				base.GetSmscAddress(out str, out num);
				stringBuilder.Append(this.EncodeSmscAddress(str, num));
			}
			stringBuilder.Append(Calc.IntToHex(this.messageFlags));
			stringBuilder.Append(Calc.IntToHex(this.messageReference));
			stringBuilder.Append(this.EncodeDestinationAddress(this.destAddress, this.destTOA));
			stringBuilder.Append(Calc.IntToHex(base.ProtocolID));
			stringBuilder.Append(Calc.IntToHex(base.DataCodingScheme));
			stringBuilder.Append(empty);
			stringBuilder.Append(Calc.IntToHex(base.UserDataLength));
			if (base.UserData != null)
			{
				stringBuilder.Append(Calc.IntToHex(base.UserData));
			}
			return stringBuilder.ToString();
		}
	}
}