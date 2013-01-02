using System;

/// <summary>
/// Represents an outgoing SMS PDU.
/// </summary>
namespace GsmComm.PduConverter
{
	public abstract class OutgoingSmsPdu : SmsPdu
	{
		private const byte TP_MTI_SMS_Deliver_Report = 0;

		private const byte TP_MTI_SMS_Submit = 1;

		private const byte TP_MTI_SMS_Command = 2;

		/// <summary>
		/// The flags for this message.
		/// </summary>
		protected OutgoingMessageFlags messageFlags;

		/// <summary>
		/// The message reference.
		/// </summary>
		protected byte messageReference;

		/// <summary>
		/// Gets or sets the message reference.
		/// </summary>
		/// <remarks><para>Represents the TP-Message-Reference octet of the PDU.</para>
		/// <para>Normally there is no need to change this property because
		/// the reference is set by the sending device.</para>.
		/// </remarks>
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
		/// Gets the message type.
		/// </summary>
		public OutgoingMessageType MessageType
		{
			get
			{
				return this.messageFlags.MessageType;
			}
		}

		/// <summary>
		/// Initializes a new <see cref="T:GsmComm.PduConverter.SmsDeliverPdu" /> instance.
		/// </summary>
		protected OutgoingSmsPdu()
		{
			this.messageReference = 0;
		}

		/// <summary>
		/// Decodes an outgoing SMS PDU stream.
		/// </summary>
		/// <param name="pdu">The PDU string to decode</param>
		/// <param name="includesSmscData">Specify true if the PDU data contains an SMSC header, otherwise false.</param>
		/// <param name="actualLength">The length of the PDU in bytes, not including the SMSC header.</param>
		/// <returns>An <see cref="T:GsmComm.PduConverter.OutgoingSmsPdu" /> object representing the decoded message.</returns>
		public static OutgoingSmsPdu Decode(string pdu, bool includesSmscData, int actualLength)
		{
			if (pdu != string.Empty)
			{
				int num = 0;
				if (includesSmscData)
				{
					int num1 = num;
					num = num1 + 1;
					byte num2 = BcdWorker.GetByte(pdu, num1);
					if (num2 > 0)
					{
						num = num + num2;
					}
				}
				int num3 = num;
				OutgoingMessageType messageType = OutgoingSmsPdu.GetMessageType(BcdWorker.GetByte(pdu, num3));
				OutgoingMessageType outgoingMessageType = messageType;
				if (outgoingMessageType != OutgoingMessageType.SmsSubmit)
				{
					throw new NotSupportedException(string.Concat("Message type ", messageType.ToString(), " recognized, but not supported by the SMS decoder."));
				}
				else
				{
					return new SmsSubmitPdu(pdu, includesSmscData, actualLength);
				}
			}
			else
			{
				throw new ArgumentException("pdu must not be an empty string.");
			}
		}

		/// <summary>
		/// Decodes an outgoing SMS PDU stream.
		/// </summary>
		/// <param name="pdu">The PDU string to decode.</param>
		/// <param name="includesSmscData">Specify true if the PDU data contains an SMSC header, otherwise false.</param>
		/// <returns>An <see cref="T:GsmComm.PduConverter.OutgoingSmsPdu" /> object representing the decoded message.</returns>
		/// <remarks>Use this method when the actual length of the message is not known.</remarks>
		public static OutgoingSmsPdu Decode(string pdu, bool includesSmscData)
		{
			return OutgoingSmsPdu.Decode(pdu, includesSmscData, -1);
		}

		private static OutgoingMessageType GetMessageType(byte flags)
		{
			OutgoingMessageType outgoingMessageType;
			int num;
			if ((flags & 2) > 0)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			byte num1 = (byte)(num * 2 + (flags & 1));
			byte num2 = num1;
			if (num2 == 0)
			{
				outgoingMessageType = OutgoingMessageType.SmsDeliverReport;
			}
			else if (num2 == 1)
			{
				outgoingMessageType = OutgoingMessageType.SmsSubmit;
			}
			else if (num2 == 2)
			{
				outgoingMessageType = OutgoingMessageType.SmsCommand;
			}
			else
			{
				string[] str = new string[5];
				str[0] = "Unknown message type ";
				str[1] = num1.ToString();
				str[2] = " (flags=";
				str[3] = flags.ToString();
				str[4] = ")";
				throw new ArgumentException(string.Concat(str), "flags");
			}
			return outgoingMessageType;
		}
	}
}