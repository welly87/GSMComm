using System;

/// <summary>
/// Implements a Concatenated Short Message Information Element (16-bit reference number)
/// </summary>
/// <remarks>This element is used to indiate that a message is split into
/// multiple parts.</remarks>
namespace GsmComm.PduConverter.SmartMessaging
{
	public class ConcatMessageElement16 : InformationElement, IConcatenationInfo
	{
		/// <summary>
		/// The Information Element Identifier (IEI).
		/// </summary>
		public const byte Identifier = 8;

		private ushort referenceNumber;

		private byte totalMessages;

		private byte currentNumber;

		/// <summary>
		/// Gets the current message number.
		/// </summary>
		public byte CurrentNumber
		{
			get
			{
				return this.currentNumber;
			}
		}

		/// <summary>
		/// Gets the current message number.
		/// </summary>
		byte GsmComm.PduConverter.SmartMessaging.IConcatenationInfo.CurrentNumber
		{
			get
			{
				return this.currentNumber;
			}
		}

		/// <summary>
		/// Gets the message reference number.
		/// </summary>
		ushort GsmComm.PduConverter.SmartMessaging.IConcatenationInfo.ReferenceNumber
		{
			get
			{
				return this.referenceNumber;
			}
		}

		/// <summary>
		/// Gets the total number of parts of the message.
		/// </summary>
		byte GsmComm.PduConverter.SmartMessaging.IConcatenationInfo.TotalMessages
		{
			get
			{
				return this.totalMessages;
			}
		}

		/// <summary>
		/// Gets the message reference number.
		/// </summary>
		public ushort ReferenceNumber
		{
			get
			{
				return this.referenceNumber;
			}
		}

		/// <summary>
		/// Gets the total number of parts of the message.
		/// </summary>
		public byte TotalMessages
		{
			get
			{
				return this.totalMessages;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmartMessaging.ConcatMessageElement16" /> class.
		/// </summary>
		/// <param name="referenceNumber">The message's reference number, must
		/// be the same in all parts of the same message.</param>
		/// <param name="totalMessages">The total number of parts of the message.</param>
		/// <param name="currentNumber">The current message number.</param>
		public ConcatMessageElement16(ushort referenceNumber, byte totalMessages, byte currentNumber)
		{
			this.referenceNumber = referenceNumber;
			this.totalMessages = totalMessages;
			this.currentNumber = currentNumber;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmartMessaging.ConcatMessageElement16" /> class.
		/// </summary>
		/// <param name="element">The information element as a byte array.</param>
		public ConcatMessageElement16(byte[] element)
		{
			if (element != null)
			{
				if (element[0] == 8)
				{
					byte num = element[1];
					if (num >= 4)
					{
						byte[] numArray = new byte[2];
						numArray[0] = element[3];
						numArray[1] = element[2];
						this.referenceNumber = BitConverter.ToUInt16(numArray, 0);
						this.totalMessages = element[4];
						this.currentNumber = element[5];
						return;
					}
					else
					{
						throw new FormatException("Information element data must be 4 bytes long.");
					}
				}
				else
				{
					throw new ArgumentException("Element is not a Concatenated Short Message Information Element (16-bit reference number).", "element");
				}
			}
			else
			{
				throw new ArgumentNullException("element");
			}
		}

		/// <summary>
		/// Returns the byte array equivalent of this instance.
		/// </summary>
		/// <returns>The byte array.</returns>
		public override byte[] ToByteArray()
		{
			byte[] bytes = BitConverter.GetBytes(this.referenceNumber);
			byte[] numArray = new byte[6];
			numArray[0] = 8;
			numArray[1] = 4;
			numArray[2] = bytes[1];
			numArray[3] = bytes[0];
			numArray[4] = this.totalMessages;
			numArray[5] = this.currentNumber;
			return numArray;
		}
	}
}