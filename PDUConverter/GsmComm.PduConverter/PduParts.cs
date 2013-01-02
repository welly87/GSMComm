using System;
using System.Text;

/// <summary>
/// Implements decoding routines for SMS PDU parts that are not implemented as separate objects.
/// </summary>
namespace GsmComm.PduConverter
{
	public static class PduParts
	{
		/// <summary>
		/// Decodes the text from 7-Bit user data.
		/// </summary>
		/// <param name="userData">The user data to decode. Must contain an encoded GSM 7-Bit default text packed into octets.</param>
		/// <returns>The decoded user data.</returns>
		public static string Decode7BitText(byte[] userData)
		{
			string septetsStr = TextDataConverter.OctetsToSeptetsStr(userData);
			return TextDataConverter.SevenBitToString(septetsStr, true);
		}

		/// <summary>
		/// Decodes an address out of a PDU string.
		/// </summary>
		/// <param name="pdu">The PDU string to use.</param>
		/// <param name="index">The index where to start in the string.</param>
		/// <param name="address">The address (phone number) read.</param>
		/// <param name="addressType">The address type of the read address.</param>
		public static void DecodeGeneralAddress(string pdu, ref int index, out string address, out byte addressType)
		{
			int num;
			int num1 = index;
			int num2 = num1;
			index = num1 + 1;
			byte num3 = BcdWorker.GetByte(pdu, num2);
			int num4 = index;
			int num5 = num4;
			index = num4 + 1;
			addressType = BcdWorker.GetByte(pdu, num5);
			if (num3 <= 0)
			{
				address = string.Empty;
				return;
			}
			else
			{
				bool flag = false;
				if (num3 % 2 != 0)
				{
					num = num3 + 1;
				}
				else
				{
					num = (int)num3;
				}
				int length = num / 2;
				if (index * 2 + length * 2 > pdu.Length - length * 2)
				{
					length = (pdu.Length - index * 2) / 2;
					flag = true;
				}
				AddressType addressType1 = new AddressType(addressType);
				if (addressType1.Ton != 5)
				{
					string bytesString = BcdWorker.GetBytesString(pdu, index, length);
					index = index + length;
					if (flag)
					{
						address = BcdWorker.DecodeSemiOctets(bytesString).Substring(0, length * 2);
						return;
					}
					else
					{
						address = BcdWorker.DecodeSemiOctets(bytesString).Substring(0, num3);
						return;
					}
				}
				else
				{
					byte[] bytes = BcdWorker.GetBytes(pdu, index, length);
					index = index + length;
					address = PduParts.Decode7BitText(bytes);
					return;
				}
			}
		}

		/// <summary>
		/// Decodes an SMSC address out of a PDU string.
		/// </summary>
		/// <param name="pdu">The PDU string to use.</param>
		/// <param name="index">The index where to start in the string.</param>
		/// <param name="address">The address (phone number) read.</param>
		/// <param name="addressType">The address type of the read address.</param>
		public static void DecodeSmscAddress(string pdu, ref int index, out string address, out byte addressType)
		{
			int num = index;
			int num1 = num;
			index = num + 1;
			byte num2 = BcdWorker.GetByte(pdu, num1);
			if (num2 <= 0)
			{
				addressType = 0;
				address = string.Empty;
				return;
			}
			else
			{
				int num3 = index;
				int num4 = num3;
				index = num3 + 1;
				byte num5 = BcdWorker.GetByte(pdu, num4);
				int num6 = num2 - 1;
				string bytesString = BcdWorker.GetBytesString(pdu, index, num6);
				index = index + num6;
				string str = BcdWorker.DecodeSemiOctets(bytesString);
				if (str.EndsWith("F") || str.EndsWith("f"))
				{
					str = str.Substring(0, str.Length - 1);
				}
				addressType = num5;
				address = str;
				return;
			}
		}

		/// <summary>
		/// Decodes text from user data in the specified data coding scheme.
		/// </summary>
		/// <param name="userData">The user data to decode. Must contain text according to the specified data coding scheme.</param>
		/// <param name="dataCodingScheme">The data coding scheme specified in the PDU.</param>
		/// <returns>The decoded user data.</returns>
		public static string DecodeText(byte[] userData, byte dataCodingScheme)
		{
			string str;
			byte alphabet = DataCodingScheme.Decode(dataCodingScheme).Alphabet;
			byte num = alphabet;
			switch (num)
			{
				case 0:
				{
					str = PduParts.Decode7BitText(userData);
					break;
				}
				case 1:
				{
                //Label0:
					str = PduParts.Decode7BitText(userData);
					break;
				}
				case 2:
				{
					str = PduParts.DecodeUcs2Text(userData);
					break;
				}
				default:
				{
                    //goto Label0;
                    str = PduParts.Decode7BitText(userData);
                    break;
				}
			}
			return str;
		}

		/// <summary>
		/// Decodes the text from UCS2 (16-Bit) user data.
		/// </summary>
		/// <param name="userData">The user data to decode. Must contain an encoded UCS2 text.</param>
		/// <returns>The decoded user data.</returns>
		public static string DecodeUcs2Text(byte[] userData)
		{
			Encoding bigEndianUnicode = Encoding.BigEndianUnicode;
			return bigEndianUnicode.GetString(userData);
		}

		/// <summary>
		/// Gets the user data out of the string.
		/// </summary>
		/// <param name="pdu">The PDU string to use.</param>
		/// <param name="index">The index where to start in the string.</param>
		/// <param name="dcs">The coding that was used to encode the data. Required to determine the proper data length.</param>
		/// <param name="userDataLength">Receives the user data length in bytes.</param>
		/// <param name="userData">Received the user data.</param>
		/// <remarks>
		/// <para>If there's no data, userDataLength will be set to 0 and userData to null.</para>
		/// <para>The decoded data might require further processing, for example 7-bit data (septets) packed
		/// into octets, that must be converted back to septets before the data can be used.</para>
		/// <para>Processing will stop at the first character that is not hex encountered or if the
		/// string ends too early. It will not change the <b>userDataLength</b> read from the string.</para>
		/// </remarks>
		public static void DecodeUserData(string pdu, ref int index, byte dcs, out byte userDataLength, out byte[] userData)
		{
			int num = index;
			int num1 = num;
			index = num + 1;
			byte num2 = BcdWorker.GetByte(pdu, num1);
			if (num2 <= 0)
			{
				userDataLength = 0;
				userData = new byte[0];
				return;
			}
			else
			{
				int remainingUserDataBytes = PduParts.GetRemainingUserDataBytes(num2, dcs);
				int num3 = BcdWorker.CountBytes(pdu) - index;
				if (num3 < remainingUserDataBytes)
				{
					remainingUserDataBytes = num3;
				}
				string bytesString = BcdWorker.GetBytesString(pdu, index, remainingUserDataBytes);
				index = index + remainingUserDataBytes;
				string empty = string.Empty;
				for (int i = 0; i < bytesString.Length / 2; i++)
				{
					string byteString = BcdWorker.GetByteString(bytesString, i);
					if (!Calc.IsHexString(byteString))
					{
						break;
					}
					empty = string.Concat(empty, byteString);
				}
				userDataLength = num2;
				userData = Calc.HexToInt(empty);
				return;
			}
		}

		/// <summary>
		/// Calculates the number of bytes that must be present in the user data portion of the PDU.
		/// </summary>
		/// <param name="dataLength">The user data length specified in the PDU.</param>
		/// <param name="dataCodingScheme">The data coding scheme specified in the PDU.</param>
		/// <returns>The number of bytes (octets) that must be present, or, if decoding the user data, the number
		/// of remaining bytes that must be read.</returns>
		/// <remarks>The <b>dataLength</b> and <b>dataCodingScheme</b> parameters are used to
		/// calculate the number of bytes that must be present in the user data.</remarks>
		internal static int GetRemainingUserDataBytes(byte dataLength, byte dataCodingScheme)
		{
			int num;
			DataCodingScheme dataCodingScheme1 = DataCodingScheme.Decode(dataCodingScheme);
			byte alphabet = dataCodingScheme1.Alphabet;
			switch (alphabet)
			{
				case 0:
				{
					num = (int)Math.Ceiling((double)dataLength * 7 / 8);
					break;
				}
				case 1:
				case 2:
				{
					num = dataLength;
					break;
				}
				default:
				{
					num = (int)Math.Ceiling((double)dataLength * 7 / 8);
					break;
				}
			}
			return num;
		}
	}
}