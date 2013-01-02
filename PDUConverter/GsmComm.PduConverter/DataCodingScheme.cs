using System;

/// <summary>
/// Indicates how the user data is encoded, this class represents the TP-DCS field.
/// </summary>
/// <remarks>
/// The TP-Data-Coding-Scheme field, defined in GSM 03.40, indicates the data coding scheme of the TP-UD field,
/// and may indicate a message class. Any reserved codings shall be assumed to be the GSM default alphabet
/// (the same as codepoint 00000000) by a receiving entity. The octet is used according to a coding group
/// which is indicated in bits 7..4
/// </remarks>
namespace GsmComm.PduConverter
{
	public abstract class DataCodingScheme
	{
		private const byte classValid = 16;

		/// <summary>
		/// Specifies no message class and 7-bit default alphabet.
		/// </summary>
		public const byte NoClass_7Bit = 0;

		/// <summary>
		/// Specifies message class 0 (immediate display) and 7-bit default alphabet.
		/// </summary>
		public const byte Class0_7Bit = 16;

		/// <summary>
		/// Specifies message class 1 (ME specific) and 7-bit default alphabet.
		/// </summary>
		public const byte Class1_7Bit = 17;

		/// <summary>
		/// Specifies message class 2 (SIM specific) and 7-bit default alphabet.
		/// </summary>
		public const byte Class2_7Bit = 18;

		/// <summary>
		/// Specifies message class 3 (TE specific) and 7-bit default alphabet.
		/// </summary>
		public const byte Class3_7Bit = 19;

		/// <summary>
		/// Specifies no message class and 8-bit data.
		/// </summary>
		public const byte NoClass_8Bit = 4;

		/// <summary>
		/// Specifies message class 0 (immediate display) and 8-bit data.
		/// </summary>
		public const byte Class0_8Bit = 20;

		/// <summary>
		/// Specifies message class 1 (ME specific) and 8-bit data.
		/// </summary>
		public const byte Class1_8Bit = 21;

		/// <summary>
		/// Specifies message class 2 (SIM specific) and 8-bit data.
		/// </summary>
		public const byte Class2_8Bit = 22;

		/// <summary>
		/// Specifies message class 3 (TE specific) and 8-bit data.
		/// </summary>
		public const byte Class3_8Bit = 23;

		/// <summary>
		/// Specifies no message class and UCS2 (16-bit) alphabet.
		/// </summary>
		public const byte NoClass_16Bit = 8;

		/// <summary>
		/// Specifies message class 0 (immediate display) and UCS2 (16-bit) alphabet.
		/// </summary>
		public const byte Class0_16Bit = 24;

		/// <summary>
		/// Specifies message class 1 (ME specific) and UCS2 (16-bit) alphabet.
		/// </summary>
		public const byte Class1_16Bit = 25;

		/// <summary>
		/// Specifies message class 2 (SIM specific) and UCS2 (16-bit) alphabet.
		/// </summary>
		public const byte Class2_16Bit = 26;

		/// <summary>
		/// Specifies message class 3 (TE specific) and UCS2 (16-bit) alphabet.
		/// </summary>
		public const byte Class3_16Bit = 27;

		/// <summary>Offset for bit 7 value.</summary>
		protected const byte bit7offset = 128;

		/// <summary>Offset for bit 6 value.</summary>
		protected const byte bit6offset = 64;

		/// <summary>Offset for bit 5 value.</summary>
		protected const byte bit5offset = 32;

		/// <summary>Offset for bit 4 value.</summary>
		protected const byte bit4offset = 16;

		/// <summary>Offset for bit 3 value.</summary>
		protected const byte bit3offset = 8;

		/// <summary>Offset for bit 2 value.</summary>
		protected const byte bit2offset = 4;

		/// <summary>Offset for bit 1 value.</summary>
		protected const byte bit1offset = 2;

		/// <summary>Offset for bit 0 value.</summary>
		protected const byte bit0offset = 1;

		private byte codingGroup;

		/// <summary>
		/// Gets the alphabet being used.
		/// </summary>
		public abstract byte Alphabet
		{
			get;
		}

		/// <summary>
		/// Gets the coding group, that tells about the further contents of the data coding scheme.
		/// </summary>
		public byte CodingGroup
		{
			get
			{
				return this.codingGroup;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.DataCodingScheme" /> class.
		/// </summary>
		/// <param name="dcs">The DCS byte to decode.</param>
		protected DataCodingScheme(byte dcs)
		{
			this.codingGroup = (byte)(dcs >> 4 & 15);
		}

		/// <summary>
		/// Decodes the given DCS byte.
		/// </summary>
		/// <param name="dcs">The DCS octet to decode.</param>
		/// <returns>An object of type <see cref="T:GsmComm.PduConverter.DataCodingScheme" /> or one of its derived classes.</returns>
		public static DataCodingScheme Decode(byte dcs)
		{
			DataCodingScheme messageWaitingDiscard;
			byte num = (byte)(dcs >> 4 & 15);
			if ((dcs & 64) != 0 || (dcs & 128) != 0)
			{
				byte num1 = num;
				switch (num1)
				{
					case 12:
					{
						messageWaitingDiscard = new MessageWaitingDiscard(dcs);
						break;
					}
					case 13:
					{
						messageWaitingDiscard = new MessageWaitingStore(dcs);
						break;
					}
					case 14:
					{
						messageWaitingDiscard = new MessageWaitingStoreUcs2(dcs);
						break;
					}
					case 15:
					{
						messageWaitingDiscard = new MessageCoding(dcs);
						break;
					}
					default:
					{
						messageWaitingDiscard = new ReservedCodingGroup(dcs);
						break;
					}
				}
			}
			else
			{
				messageWaitingDiscard = new GeneralDataCoding(dcs);
			}
			return messageWaitingDiscard;
		}

		/// <summary>
		/// Lists the available alphabets within a general data coding indication DCS.
		/// </summary>
		public enum Alphabets : byte
		{
			DefaultAlphabet,
			EightBit,
			Ucs2,
			Reserved
		}

		/// <summary>
		/// Data coding/message class. Members can be combined.
		/// </summary>
		/// <remarks>At least a "Group" member must be specified.</remarks>
		[Flags]
		public enum DataCoding
		{
			Alpha7BitDefault = 0,
			Class0 = 0,
			Class1 = 1,
			Class2 = 2,
			Class3 = 3,
			Alpha8Bit = 4,
			Group_DataCoding = 240
		}

		/// <summary>
		/// General data coding indication. Members can be combined.
		/// </summary>
		[Flags]
		public enum GeneralCoding : byte
		{
			Uncompressed,
			Alpha7BitDefault,
			NoClass,
			Alpha8Bit,
			Alpha16Bit,
			AlphaReserved,
			Class0,
			Class1,
			Class2,
			Class3,
			Compressed
		}

		/// <summary>
		/// Lists the available message codings within a data coding/message class DCS.
		/// </summary>
		public enum MessageCodings : byte
		{
			DefaultAlphabet,
			EightBit
		}

		/// <summary>
		/// Message waiting indication. Members can be combined.
		/// </summary>
		/// <remarks>At least a "Group" member must be specified.</remarks>
		[Flags]
		public enum MessageWaiting : byte
		{
			SetIndicationInactive,
			VoicemailMsgWaiting,
			FaxMsgWaiting,
			EMailMsgWaiting,
			OtherMsgWaiting,
			SetIndicationActive,
			Group_Discard_7BitDefault,
			Group_Store_7BitDefault,
			Group_Store_16Bit
		}
	}
}