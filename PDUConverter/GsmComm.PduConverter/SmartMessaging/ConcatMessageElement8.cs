using System;

/// <summary>
/// Implements a Concatenated Short Message Information Element (8-bit reference number)
/// </summary>
/// <remarks>This element is used to indiate that a message is split into
/// multiple parts.</remarks>
namespace GsmComm.PduConverter.SmartMessaging
{
	public class ConcatMessageElement8 : InformationElement, IConcatenationInfo
	{
		/// <summary>
		/// The Information Element Identifier (IEI).
		/// </summary>
		public const byte Identifier = 0;

		private byte referenceNumber;

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
		public byte ReferenceNumber
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
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmartMessaging.ConcatMessageElement8" /> class.
		/// </summary>
		/// <param name="referenceNumber">The message's reference number, must
		/// be the same in all parts of the same message.</param>
		/// <param name="totalMessages">The total number of parts of the message.</param>
		/// <param name="currentNumber">The current message number.</param>
		public ConcatMessageElement8(byte referenceNumber, byte totalMessages, byte currentNumber)
		{
			this.referenceNumber = referenceNumber;
			this.totalMessages = totalMessages;
			this.currentNumber = currentNumber;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmartMessaging.ConcatMessageElement8" /> class.
		/// </summary>
		/// <param name="element">The information element as a byte array.</param>
		public ConcatMessageElement8(byte[] element)
		{
			if (element != null)
			{
				if (element[0] == 0)
				{
					byte num = element[1];
					if (num >= 3)
					{
						this.referenceNumber = element[2];
						this.totalMessages = element[3];
						this.currentNumber = element[4];
						return;
					}
					else
					{
						throw new FormatException("Information element data must be 3 bytes long.");
					}
				}
				else
				{
					throw new ArgumentException("Element is not a Concatenated Short Message Information Element (8-bit reference number).", "element");
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
			byte[] numArray = new byte[5];
			numArray[1] = 3;
			numArray[2] = this.referenceNumber;
			numArray[3] = this.totalMessages;
			numArray[4] = this.currentNumber;
			return numArray;
		}
	}
}