using System;
using System.Text;

/// <summary>
/// Provides the base for an SMS PDU.
/// </summary>
namespace GsmComm.PduConverter
{
	public abstract class SmsPdu : ITimestamp
	{
		private const int maxSeptets = 160;

		private const byte maxOctets = 140;

		/// <summary>
		/// Gets the maximum message text length in septets.
		/// </summary>
		public const int MaxTextLength = 160;

		/// <summary>
		/// Gets the maximum Unicode message text length in characters.
		/// </summary>
		public const int MaxUnicodeTextLength = 70;

		private byte smscTOA;

		private string smscAddress;

		private byte PID;

		private byte DCS;

		private byte userDataLength;

		private byte[] userData;

		private int constructLength;

		/// <summary>
		/// Gets the length of the actual PDU data part in bytes. That is,
		/// without the SMSC header.
		/// </summary>
		public int ActualLength
		{
			get
			{
				return this.ToString(true).Length / 2;
			}
		}

		/// <summary>
		/// Gets the number of characters that have been actually been used for decoding upon construction.
		/// </summary>
		public int ConstructLength
		{
			get
			{
				return this.constructLength;
			}
			protected set
			{
				this.constructLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the data coding scheme.
		/// </summary>
		/// <remarks><para>Represents the TP-DCS octet of the PDU.</para>
		/// <para>The data coding scheme specifies how the data is coded
		/// and may also specify a message class.</para></remarks>
		public byte DataCodingScheme
		{
			get
			{
				return this.DCS;
			}
			set
			{
				this.DCS = value;
			}
		}

		/// <summary>
		/// Gets or sets the protocol identifier.
		/// </summary>
		/// <remarks>Represents the TP-PID octet of the PDU.</remarks>
		public byte ProtocolID
		{
			get
			{
				return this.PID;
			}
			set
			{
				this.PID = value;
			}
		}

		/// <summary>
		/// Gets or sets the SMSC address.
		/// </summary>
		/// <remarks>
		/// <para>When setting the property: Also the <see cref="P:GsmComm.PduConverter.SmsPdu.SmscAddressType" /> property will be set,
		/// attempting to autodetect the address type.</para>
		/// <para>When getting the property: The address may be extended with address-type
		/// specific prefixes or other chraracters.</para>
		/// </remarks>
		public string SmscAddress
		{
			get
			{
				return this.CreateAddressOfType(this.smscAddress, this.smscTOA);
			}
			set
			{
				byte num = 0;
				string str = null;
				this.FindTypeOfAddress(value, out num, out str);
				this.smscAddress = str;
				this.smscTOA = num;
			}
		}

		/// <summary>
		/// Gets the type of the SMSC address.
		/// </summary>
		/// <remarks>
		/// <para>Represents the Type-of-Address octets for the SMSC address of the PDU.</para>
		/// </remarks>
		public byte SmscAddressType
		{
			get
			{
				return this.smscTOA;
			}
		}

		/// <summary>
		/// Gets the total length of the PDU string in bytes.
		/// </summary>
		public int TotalLength
		{
			get
			{
				return this.ToString().Length / 2;
			}
		}

		/// <summary>
		/// Gets the user data.
		/// </summary>
		/// <remarks>
		/// <para>Represents the TP-User-Data octet of the PDU.</para>
		/// </remarks>
		public byte[] UserData
		{
			get
			{
				return this.userData;
			}
		}

		/// <summary>
		/// Gets or sets if a user data header is present.
		/// </summary>
		public abstract bool UserDataHeaderPresent
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the user data length.
		/// </summary>
		/// <remarks>
		/// <para>Represents the TP-User-Data-Length octet of the PDU.</para>
		/// <para>The <see cref="P:GsmComm.PduConverter.SmsPdu.UserDataLength" /> does not necessarily match the number
		/// of bytes in the <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" /> because it may be further encoded.</para>
		/// </remarks>
		public byte UserDataLength
		{
			get
			{
				return this.userDataLength;
			}
		}

		/// <summary>
		/// Gets or sets the user data text (i.e. the message text).
		/// </summary>
		/// <remarks>
		/// <para>This property supports automatic encoding and decoding of text from and to the GSM 7-bit default
		/// alphabet and the UCS2 charset. For this to work properly, the <see cref="P:GsmComm.PduConverter.SmsPdu.DataCodingScheme" />
		/// property must be set correctly before setting the UserDataText.</para>
		/// <para>For all other data with other alphabets or special data codings, the <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" /> property
		/// must be used to get or set the data.</para>
		/// <para>Setting this property also sets the <see cref="P:GsmComm.PduConverter.SmsPdu.UserDataLength" /> property accordingly.</para>
		/// </remarks>
		public string UserDataText
		{
			get
			{
				return PduParts.DecodeText(this.userData, this.DCS);
			}
			set
			{
                DataCodingScheme dataCodingScheme = GsmComm.PduConverter.DataCodingScheme.Decode(this.DCS);
				if (dataCodingScheme.Alphabet != 2)
				{
					this.Encode7BitText(value);
					return;
				}
				else
				{
					this.EncodeUcs2Text(value);
					return;
				}
			}
		}

		/// <summary>
		/// Initializes a new <see cref="T:GsmComm.PduConverter.SmsPdu" /> instance using default values.
		/// </summary>
		protected SmsPdu()
		{
			this.SmscAddress = string.Empty;
			this.PID = 0;
			this.DCS = 0;
			this.userDataLength = 0;
			this.userData = null;
		}

		/// <summary>
		/// Adds a user data header to an existing user data.
		/// </summary>
		/// <param name="header">The user data header to add.</param>
		/// <exception cref="T:System.InvalidOperationException">There is already a user data header present in this message.</exception>
		/// <exception cref="T:System.ArgumentException">The resulting total of user data header and existing user data exceeds the allowed
		/// maximum data length.</exception>
		/// <remarks>
		/// <para>The user data must already be set before adding a user data header.</para>
		/// <para>Adding a user data header reduces the available space for the remaining user data. If the resulting total of
		/// user data header and existing user data exceeds allowed maximum data length, an exception is raised.</para>
		/// </remarks>
		public void AddUserDataHeader(byte[] header)
		{
			int num;
			if (this.UserDataHeaderPresent)
			{
				throw new InvalidOperationException("There is already a user data header present in this message.");
			}
			else
			{
				int length = (int)header.Length;
				byte num1 = (byte)((double)length * 8 / 7);
				DataCodingScheme dataCodingScheme = GsmComm.PduConverter.DataCodingScheme.Decode(this.DCS);
				if (dataCodingScheme.Alphabet != 0)
				{
					if (dataCodingScheme.Alphabet == 1 || dataCodingScheme.Alphabet == 2)
					{
						num = length + this.userDataLength;
					}
					else
					{
						num = num1 + this.userDataLength;
					}
				}
				else
				{
					num = num1 + this.userDataLength;
				}
				byte[] numArray = new byte[(int)header.Length + (int)this.userData.Length];
				header.CopyTo(numArray, 0);
				this.userData.CopyTo(numArray, (int)header.Length);
				this.SetUserData(numArray, (byte)num);
				this.UserDataHeaderPresent = true;
				return;
			}
		}

		/// <summary>
		/// Modifies an address to make it look like the specified address type.
		/// </summary>
		/// <param name="address">The address (phone number).</param>
		/// <param name="type">The address type.</param>
		/// <returns>The modified address.</returns>
		/// <remarks>If the address can't be modified, the original string is returned.</remarks>
		protected string CreateAddressOfType(string address, byte type)
		{
			if (!(address != string.Empty) || type != 145 || address.StartsWith("+"))
			{
				return address;
			}
			else
			{
				return string.Concat("+", address);
			}
		}

		/// <summary>
		/// Decodes the text from 7-Bit user data in this instance.
		/// </summary>
		/// <returns>The decoded <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" />.</returns>
		/// <remarks>This method assumes that the <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" /> property contains an encoded
		/// GSM 7-Bit default text packed into octets. If <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" /> contains something different,
		/// the results are not defined.</remarks>
		protected string Decode7BitText()
		{
			return PduParts.Decode7BitText(this.userData);
		}

		/// <summary>
		/// Decodes the text from UCS2 (16-Bit) user data in this instance.
		/// </summary>
		/// <returns>The decoded <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" />.</returns>
		/// <remarks>This method assumes that the <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" /> property contains an encoded
		/// UCS2 text. If <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" /> contains something different, the results are not defined.
		/// </remarks>
		protected string DecodeUcs2Text()
		{
			return PduParts.DecodeUcs2Text(this.userData);
		}

		/// <summary>
		/// Encodes the specified text as 7-Bit user data in this instance.
		/// </summary>
		/// <param name="text">The text to encode.</param>
		/// <remarks>The text is converted to the GSM 7-Bit default alphabet first, then it is packed into octets.
		/// The final result is saved in the properties <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" /> and <see cref="P:GsmComm.PduConverter.SmsPdu.UserDataLength" />.
		/// </remarks>
		/// <exception cref="T:System.ArgumentException">Text is too long.</exception>
		protected void Encode7BitText(string text)
		{
			string str = TextDataConverter.StringTo7Bit(text);
			int length = str.Length;
			if (length <= 160)
			{
				this.SetUserData(TextDataConverter.SeptetsToOctetsInt(str), (byte)length);
				return;
			}
			else
			{
				string[] strArrays = new string[5];
				strArrays[0] = "Text is too long. A maximum of ";
				int num = 160;
				strArrays[1] = num.ToString();
				strArrays[2] = " resulting septets is allowed. The current input results in ";
				strArrays[3] = length.ToString();
				strArrays[4] = " septets.";
				throw new ArgumentException(string.Concat(strArrays));
			}
		}

		/// <summary>
		/// Encodes the specified text as UCS2 (16-Bit) user data in this instance.
		/// </summary>
		/// <param name="text">The text to encode.</param>
		/// <remarks>The text is converted to the UCS2 character set. The result is saved in the properties
		/// <see cref="P:GsmComm.PduConverter.SmsPdu.UserData" /> and <see cref="P:GsmComm.PduConverter.SmsPdu.UserDataLength" />.</remarks>
		protected void EncodeUcs2Text(string text)
		{
			Encoding bigEndianUnicode = Encoding.BigEndianUnicode;
			byte[] bytes = bigEndianUnicode.GetBytes(text);
			int length = (int)bytes.Length;
			if (length <= 140)
			{
				this.SetUserData(bytes, (byte)length);
				return;
			}
			else
			{
				string[] str = new string[5];
				str[0] = "Text is too long. A maximum of ";
				byte num = 140;
				str[1] = num.ToString();
				str[2] = " resulting octets is allowed. The current input results in ";
				str[3] = length.ToString();
				str[4] = " octets.";
				throw new ArgumentException(string.Concat(str));
			}
		}

		/// <summary>
		/// Determines the address type.
		/// </summary>
		/// <param name="address">The address (phone number).</param>
		/// <param name="type">The detected address type.</param>
		/// <param name="useThisAddress">The modified address that can be directly used for communication.</param>
		/// <example>If you use address "+4812345678" the resulting type will be 0x91 and useThisAddress
		/// will be "4812345678". Call <see cref="M:GsmComm.PduConverter.SmsPdu.CreateAddressOfType(System.String,System.Byte)" /> to recreate the original address.</example>
		protected void FindTypeOfAddress(string address, out byte type, out string useThisAddress)
		{
			byte num = default(byte);
			if (address == string.Empty)
			{
				useThisAddress = address;
				type = 0;
				return;
			}
			else
			{
				while (address.StartsWith("+"))
				{
					num = 145;
					address = address.Substring(1);
				}
				useThisAddress = address;
				type = num;
				return;
			}
		}

		/// <summary>
		/// Modifies the message text so that it is safe to be sent via GSM 7-Bit default encoding.
		/// </summary>
		/// <param name="data">The message text.</param>
		/// <returns>The converted message text.</returns>
		/// <remarks>Replaces invalid characters in the text and truncates it to the maximum allowed length.</remarks>
		public static string GetSafeText(string data)
		{
			bool flag = false;
			bool flag1 = false;
			return SmsPdu.GetSafeText(data, out flag, out flag1);
		}

		/// <summary>
		/// Modifies the message text so that it is safe to be sent via GSM 7-Bit default encoding.
		/// </summary>
		/// <param name="data">The message text.</param>
		/// <param name="charsCorrected">Will be set to true if the message length was corrected.</param>
		/// <param name="lengthCorrected">Will be set to true if one or more characters were replaced.</param>
		/// <returns>The converted message text.</returns>
		/// <remarks>Replaces invalid characters in the text and truncates it to the maximum allowed length.</remarks>
		public static string GetSafeText(string data, out bool lengthCorrected, out bool charsCorrected)
		{
			string str;
			string str1;
			bool flag = false;
			lengthCorrected = false;
			charsCorrected = false;
			if (data.Length <= 160)
			{
				str = data;
			}
			else
			{
				str = data.Substring(0, 160);
				lengthCorrected = true;
			}
			do
			{
				str1 = TextDataConverter.StringTo7Bit(str, false, out flag);
				if (flag)
				{
					charsCorrected = true;
				}
				if (str1.Length <= 160)
				{
					continue;
				}
				str = data.Substring(0, str.Length - 1);
				lengthCorrected = true;
			}
			while (str1.Length > 160);
			string str2 = TextDataConverter.SevenBitToString(str1);
			return str2;
		}

		/// <summary>
		/// Gets the SMSC address and the type as it is saved internally.
		/// </summary>
		/// <param name="address">The SMSC address.</param>
		/// <param name="addressType">The address type of <b>address</b>.</param>
		public void GetSmscAddress(out string address, out byte addressType)
		{
			address = this.smscAddress;
			addressType = this.smscTOA;
		}

		/// <summary>
		/// Gets the length in septets of the specified text.
		/// </summary>
		/// <param name="text">The text the get the length for.</param>
		/// <returns>The text length.</returns>
		public static int GetTextLength(string text)
		{
			return TextDataConverter.StringTo7Bit(text).Length;
		}

		/// <summary>
		/// In derived classes, returns the relevant timestamp for the message.
		/// </summary>
		/// <returns>The timestamp.</returns>
		/// <remarks>If the message type does not have a relevant timestamp,
		/// it returns <see cref="F:GsmComm.PduConverter.SmsTimestamp.None" /></remarks>
		public abstract SmsTimestamp GetTimestamp();

		/// <summary>
		/// Extracts the user data header out of the user data.
		/// </summary>
		/// <returns>A byte array containing the extracted header.</returns>
		/// <exception cref="T:System.InvalidOperationException">There is no user data header is present in this message.</exception>
		/// <remarks>Use <see cref="P:GsmComm.PduConverter.SmsPdu.UserDataHeaderPresent" /> to determine whether a user data header is present.</remarks>
		public byte[] GetUserDataHeader()
		{
			if (!this.UserDataHeaderPresent)
			{
				throw new InvalidOperationException("There is no user data header is present in this message.");
			}
			else
			{
				byte num = this.userData[0];
				num = (byte)(num + 1);
				byte[] numArray = new byte[num];
				Array.Copy(this.userData, numArray, num);
				return numArray;
			}
		}

		/// <summary>
		/// Extracts the section of the user data that is not occupied by the user data header
		/// and returns it as text.
		/// </summary>
		/// <returns>A string containing the extracted text.</returns>
		/// <exception cref="T:System.InvalidOperationException">There is no user data header is present in this message.</exception>
		/// <remarks>Use <see cref="P:GsmComm.PduConverter.SmsPdu.UserDataHeaderPresent" /> to determine whether a user data header is present.</remarks>
		public string GetUserDataTextWithoutHeader()
		{
			byte[] userDataWithoutHeader = this.GetUserDataWithoutHeader();
			return PduParts.DecodeText(userDataWithoutHeader, this.DCS);
		}

		/// <summary>
		/// Extracts the section of the user data that is not occupied by the user data header.
		/// </summary>
		/// <returns>A byte array containing the extracted data.</returns>
		/// <exception cref="T:System.InvalidOperationException">There is no user data header is present in this message.</exception>
		/// <remarks>Use <see cref="P:GsmComm.PduConverter.SmsPdu.UserDataHeaderPresent" /> to determine whether a user data header is present.</remarks>
		public byte[] GetUserDataWithoutHeader()
		{
			if (!this.UserDataHeaderPresent)
			{
				throw new InvalidOperationException("There is no user data header is present in this message.");
			}
			else
			{
				byte num = this.userData[0];
				num = (byte)(num + 1);
				int length = (int)this.userData.Length - num;
				byte[] numArray = new byte[length];
				Array.Copy(this.userData, num, numArray, 0, length);
				return numArray;
			}
		}

		/// <summary>
		/// Checks if the user data portion of the PDU is complete.
		/// </summary>
		/// <returns>true if all data is available, false otherwise.</returns>
		/// <remarks>This method can be used to verify that the user data has not been truncated or otherwise
		/// invalidated.</remarks>
		public bool IsUserDataComplete()
		{
			int remainingUserDataBytes = PduParts.GetRemainingUserDataBytes(this.userDataLength, this.DCS);
			return (int)this.userData.Length >= remainingUserDataBytes;
		}

		/// <summary>
		/// Sets the SMSC address and type directly without attempting to
		/// autodetect the type.
		/// </summary>
		/// <param name="address">The SMSC address.</param>
		/// <param name="addressType">The address type of <b>address</b>.</param>
		public void SetSmscAddress(string address, byte addressType)
		{
			this.smscAddress = address;
			this.smscTOA = addressType;
		}

		/// <summary>
		/// Sets the user data using raw octets.
		/// </summary>
		/// <param name="data">The user data directly as a byte array.</param>
		/// <param name="dataLength">The length of the data. Note that this is not necessarily
		/// the number of bytes in the array, the length depends on the data coding.</param>
		/// <exception cref="T:System.ArgumentException">UserData is too long, more than 140 octets were passed</exception>
		/// <remarks>Assumes that raw octets are passed. Use the <see cref="P:GsmComm.PduConverter.SmsPdu.UserDataText" /> property
		/// if you want to pass text.</remarks>
		public void SetUserData(byte[] data, byte dataLength)
		{
			if ((int)data.Length <= 140)
			{
				this.userData = data;
				this.userDataLength = dataLength;
				return;
			}
			else
			{
				string[] str = new string[5];
				str[0] = "User data is too long. A maximum of ";
				byte num = 140;
				str[1] = num.ToString();
				str[2] = " octets is allowed. ";
				int length = (int)data.Length;
				str[3] = length.ToString();
				str[4] = " octets were passed.";
				throw new ArgumentException(string.Concat(str));
			}
		}

		/// <summary>
		/// In derived classes, converts the value of this instance into a string.
		/// </summary>
		/// <param name="excludeSmscData">If true, excludes the SMSC header, otherwise it is included.</param>
		/// <returns>The encoded string.</returns>
		public abstract string ToString(bool excludeSmscData);

		/// <summary>
		/// Converts the value of this instance into a string, including SMSC header.
		/// </summary>
		/// <returns>The encoded string.</returns>
		public override string ToString()
		{
			return this.ToString(false);
		}
	}
}