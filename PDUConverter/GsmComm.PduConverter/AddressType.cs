using System;

/// <summary>
/// Indicates the format of a phone number.
/// </summary>
/// <remarks>
/// The most common value of this octet is 91 hex (10010001 bin), which indicates international format.
/// A phone number in international format looks like 46708251358 (where the country code is 46).
/// In the national (or unknown) format the same phone number would look like 0708251358. The international
/// format is the most generic, and it has to be accepted also when the message is destined to a recipient
/// in the same country as the MSC or as the SGSN. 
/// </remarks>
namespace GsmComm.PduConverter
{
	public class AddressType
	{
		/// <summary>
		/// Unknown type of number and numbering plan.
		/// </summary>
		public const byte Unknown = 0;

		/// <summary>
		/// Unknown type of number, telephone numbering plan.
		/// </summary>
		public const byte UnknownPhone = 129;

		/// <summary>
		/// International number, telephone numbering plan.
		/// </summary>
		public const byte InternationalPhone = 145;

		private bool bit7;

		private byte ton;

		private byte npi;

		/// <summary>
		/// The Numbering Plan Identification.
		/// </summary>
		/// <remarks>
		/// The Numbering-plan-identification applies for Type-of-number = 000, 001 and 010.
		/// For Type-of-number = 101 bits 3,2,1,0 are reserved and shall be transmitted as 0000.
		/// Note that for addressing any of the entities SC, MSC, SGSN or MS, Numbering-plan-identification = 0001
		/// will always be used. However, for addressing the SME, any specified Numbering-plan-identification
		/// value may be used.
		/// </remarks>
		public byte Npi
		{
			get
			{
				return this.npi;
			}
			set
			{
				this.npi = value;
			}
		}

		/// <summary>
		/// The Type of number.
		/// </summary>
		public byte Ton
		{
			get
			{
				return this.ton;
			}
			set
			{
				this.ton = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public AddressType()
		{
			this.bit7 = true;
			this.ton = 0;
			this.npi = 0;
		}

		/// <summary>
		/// Initializes a new instance of the class using the given value.
		/// </summary>
		/// <param name="toa">The Type-of-Address octet to initialize the object with.</param>
		public AddressType(byte toa)
		{
			this.bit7 = (toa & 128) > 0;
			this.ton = (byte)(toa >> 4 & 7);
			this.npi = (byte)(toa & 15);
		}

		public static implicit operator AddressType(byte toa)
		{
			return new AddressType(toa);
		}

		public static implicit operator Byte(AddressType a)
		{
			return a.ToByte();
		}

		/// <summary>
		/// Returns the byte equivalent of this instance.
		/// </summary>
		/// <returns>The byte value.</returns>
		public byte ToByte()
		{
			int num;
			if (this.bit7)
			{
				num = 128;
			}
			else
			{
				num = 0;
			}
			byte num1 = (byte)(num | this.ton << 4 | this.npi);
			return num1;
		}

		/// <summary>
		/// Indicates the Numbering Plan Identification (NPI) of the phone number.
		/// </summary>
		public enum NumberingPlan : byte
		{
			Unknown,
			Telephone,
			Data,
			Telex,
			National,
			Private,
			Ermes,
			Reserved
		}

		/// <summary>
		/// Indicates the type of the phone number (TON).
		/// </summary>
		public enum TypeOfNumber : byte
		{
			Unknown,
			International,
			National,
			NetworkSpecific,
			Subscriber,
			Alphanumeric,
			Abbreviated,
			Reserved
		}
	}
}