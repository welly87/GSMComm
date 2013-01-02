using System;

/// <summary>
/// Represents an incoming SMS PDU.
/// </summary>
namespace GsmComm.PduConverter
{
	public abstract class IncomingSmsPdu : SmsPdu
	{
		private const byte TP_MTI_SMS_Deliver = 0;

		private const byte TP_MTI_SMS_Submit_Report = 1;

		private const byte TP_MTI_SMS_Status_Report = 2;

		/// <summary>
		/// The flags for this message.
		/// </summary>
		protected IncomingMessageFlags messageFlags;

		/// <summary>
		/// Gets the message type.
		/// </summary>
		public IncomingMessageType MessageType
		{
			get
			{
				return this.messageFlags.MessageType;
			}
		}

		protected IncomingSmsPdu()
		{
		}

		/// <summary>
		/// Decodes an incoming SMS PDU stream.
		/// </summary>
		/// <param name="pdu">The PDU string to decode.</param>
		/// <param name="includesSmscData">Specify true if the PDU data contains an SMSC header, otherwise false.</param>
		/// <param name="actualLength">The size of the PDU data in bytes, not counting the SMSC header. Set to -1 if unknown.</param>
		/// <returns>An <see cref="T:GsmComm.PduConverter.IncomingSmsPdu" /> object representing the decoded message.</returns>
		public static IncomingSmsPdu Decode(string pdu, bool includesSmscData, int actualLength)
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
				IncomingMessageType messageType = IncomingSmsPdu.GetMessageType(BcdWorker.GetByte(pdu, num3));
				IncomingMessageType incomingMessageType = messageType;
				switch (incomingMessageType)
				{
					case IncomingMessageType.SmsDeliver:
					{
						return new SmsDeliverPdu(pdu, includesSmscData, actualLength);
					}
					case IncomingMessageType.SmsStatusReport:
					{
						return new SmsStatusReportPdu(pdu, includesSmscData, actualLength);
					}
				}
				throw new NotSupportedException(string.Concat("Message type ", messageType.ToString(), " recognized, but not supported by the SMS decoder."));
			}
			else
			{
				throw new ArgumentException("pdu must not be an empty string.");
			}
		}

		/// <summary>
		/// Decodes an incoming SMS PDU stream.
		/// </summary>
		/// <param name="pdu">The PDU string to decode.</param>
		/// <param name="includesSmscData">Specify true if the PDU data contains an SMSC header, otherwise false.</param>
		/// <returns>An <see cref="T:GsmComm.PduConverter.IncomingSmsPdu" /> object representing the decoded message.</returns>
		/// <remarks>Use this overload only if you do not know the size of the PDU data.</remarks>
		public static IncomingSmsPdu Decode(string pdu, bool includesSmscData)
		{
			return IncomingSmsPdu.Decode(pdu, includesSmscData, -1);
		}

		private static IncomingMessageType GetMessageType(byte flags)
		{
			IncomingMessageType incomingMessageType;
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
				incomingMessageType = IncomingMessageType.SmsDeliver;
			}
			else if (num2 == 1)
			{
				incomingMessageType = IncomingMessageType.SmsSubmitReport;
			}
			else if (num2 == 2)
			{
				incomingMessageType = IncomingMessageType.SmsStatusReport;
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
			return incomingMessageType;
		}
	}
}