using GsmComm.PduConverter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text;

/// <summary>
/// Creates messages based on Nokia's Smart Messaging specification and related messages.
/// </summary>
namespace GsmComm.PduConverter.SmartMessaging
{
	public class SmartMessageFactory
	{
		private static ushort refNumber;

		/// <summary>
		/// Represents an empty operator logo that can be used to remove an operator logo.
		/// It depends on the phone model whether it works.
		/// </summary>
		public readonly static byte[] EmptyOperatorLogo;

		static SmartMessageFactory()
		{
			SmartMessageFactory.refNumber = 1;
			SmartMessageFactory.EmptyOperatorLogo = SmartMessageFactory.CreateOperatorLogo(new OtaBitmap((Bitmap)null), "000", "00");
		}

		public SmartMessageFactory()
		{
		}

		/// <summary>
		/// Calculates the next reference number.
		/// </summary>
		/// <remarks>Returns the current number and then increments it by one.
		/// If the new number would exceed 65535, it it reset to 1.</remarks>
		/// <returns>The next reference number.</returns>
		protected static ushort CalcNextRefNumber()
		{
			ushort num;
			lock (typeof(SmartMessageFactory))
			{
				num = SmartMessageFactory.refNumber;
				if (SmartMessageFactory.refNumber != 65535)
				{
					SmartMessageFactory.refNumber = (ushort)(SmartMessageFactory.refNumber + 1);
				}
				else
				{
					SmartMessageFactory.refNumber = 1;
				}
			}
			return num;
		}

		/// <summary>
		/// Creates a concatenated text message.
		/// </summary>
		/// <param name="userDataText">The message text.</param>
		/// <param name="destinationAddress">The message's destination address.</param>
		/// <returns>A set of <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> objects that represent the message.</returns>
		/// <remarks>
		/// <para>A concatenated message makes it possible to exceed the maximum length of a normal message,
		/// created by splitting the message data into multiple parts.</para>
		/// <para>Concatenated messages are also known as long or multi-part messages.</para>
		/// <para>The userDataText is converted to the GSM 7-bit default alphabet automatically.</para>
		/// <para>If no concatenation is necessary, a single, non-concatenated <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> object is created.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentException"><para>userDataText is so long that it would create more than 255 message parts.</para></exception>
		public static SmsSubmitPdu[] CreateConcatTextMessage(string userDataText, string destinationAddress)
		{
			return SmartMessageFactory.CreateConcatTextMessage(userDataText, false, destinationAddress);
		}

		/// <summary>
		/// Creates a concatenated text message.
		/// </summary>
		/// <param name="userDataText">The message text.</param>
		/// <param name="unicode">Specifies if the userDataText is to be encoded as Unicode. If not, the GSM 7-bit default alphabet is used.</param>
		/// <param name="destinationAddress">The message's destination address.</param>
		/// <returns>A set of <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> objects that represent the message.</returns>
		/// <remarks>
		/// <para>A concatenated message makes it possible to exceed the maximum length of a normal message,
		/// created by splitting the message data into multiple parts.</para>
		/// <para>Concatenated messages are also known as long or multi-part messages.</para>
		/// <para>If no concatenation is necessary, a single, non-concatenated <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> object is created.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentException"><para>userDataText is so long that it would create more than 255 message parts.</para></exception>
		public static SmsSubmitPdu[] CreateConcatTextMessage(string userDataText, bool unicode, string destinationAddress)
		{
			string str;
			int length = 0;
			int num;
			byte[] bytes;
			SmsSubmitPdu smsSubmitPdu;
			int num1;
			byte num2;
			if (unicode)
			{
				num1 = 70;
			}
			else
			{
				num1 = 160;
			}
			int num3 = num1;
			if (unicode)
			{
				str = userDataText;
			}
			else
			{
				str = TextDataConverter.StringTo7Bit(userDataText);
			}
			if (str.Length <= num3)
			{
				if (unicode)
				{
					smsSubmitPdu = new SmsSubmitPdu(userDataText, destinationAddress, 8);
				}
				else
				{
					smsSubmitPdu = new SmsSubmitPdu(userDataText, destinationAddress);
				}
				SmsSubmitPdu[] smsSubmitPduArray = new SmsSubmitPdu[1];
				smsSubmitPduArray[0] = smsSubmitPdu;
				return smsSubmitPduArray;
			}
			else
			{
				ConcatMessageElement16 concatMessageElement16 = new ConcatMessageElement16(0, 0, 0);
				byte length1 = (byte)((int)SmartMessageFactory.CreateUserDataHeader(concatMessageElement16).Length);
				byte num4 = (byte)((double)length1 / 7 * 8);
				if (unicode)
				{
					num2 = length1;
				}
				else
				{
					num2 = num4;
				}
				byte num5 = num2;
				StringCollection stringCollections = new StringCollection();
				for (int i = 0; i < str.Length; i = i + length)
				{
					if (!unicode)
					{
						if (str.Length - i < num3 - num5)
						{
							length = str.Length - i;
						}
						else
						{
							length = num3 - num5;
						}
					}
					else
					{
						if (str.Length - i < (num3 * 2 - num5) / 2)
						{
							length = str.Length - i;
						}
						else
						{
							length = (num3 * 2 - num5) / 2;
						}
					}
					string str1 = str.Substring(i, length);
					stringCollections.Add(str1);
				}
				if (stringCollections.Count <= 255)
				{
					SmsSubmitPdu[] smsSubmitPduArray1 = new SmsSubmitPdu[stringCollections.Count];
					ushort num6 = SmartMessageFactory.CalcNextRefNumber();
					byte num7 = 0;
					for (int j = 0; j < stringCollections.Count; j++)
					{
						num7 = (byte)(num7 + 1);
						ConcatMessageElement16 concatMessageElement161 = new ConcatMessageElement16(num6, (byte)stringCollections.Count, num7);
						byte[] numArray = SmartMessageFactory.CreateUserDataHeader(concatMessageElement161);
						if (unicode)
						{
							Encoding bigEndianUnicode = Encoding.BigEndianUnicode;
							bytes = bigEndianUnicode.GetBytes(stringCollections[j]);
							num = (int)bytes.Length;
						}
						else
						{
							bytes = TextDataConverter.SeptetsToOctetsInt(stringCollections[j]);
							num = stringCollections[j].Length;
						}
						SmsSubmitPdu smsSubmitPdu1 = new SmsSubmitPdu();
						smsSubmitPdu1.DestinationAddress = destinationAddress;
						if (unicode)
						{
							smsSubmitPdu1.DataCodingScheme = 8;
						}
						smsSubmitPdu1.SetUserData(bytes, (byte)num);
						smsSubmitPdu1.AddUserDataHeader(numArray);
						smsSubmitPduArray1[j] = smsSubmitPdu1;
					}
					return smsSubmitPduArray1;
				}
				else
				{
					throw new ArgumentException("A concatenated message must not have more than 255 parts.", "userDataText");
				}
			}
		}

		/// <summary>
		/// Creates an operator logo.
		/// </summary>
		/// <param name="otaBitmap">The OTA bitmap to use as the logo. Maximum size is 72x14 pixels.</param>
		/// <param name="mobileCountryCode">MCC. The operator's country code. Must be 3 digits long.</param>
		/// <param name="mobileNetworkCode">MNC. The operator's network code. Must be 2 digits long.</param>
		/// <returns>A byte array containing the generated operator logo.</returns>
		/// <exception cref="T:System.ArgumentNullException">otaBitmap is null.</exception>
		/// <exception cref="T:System.ArgumentException"><para>mobileCountryCode is not 3 digits long.</para><para> -or- </para>
		/// <para>mobileNetworkCode is not 2 digits long.</para>
		/// <para> -or- </para><para>The bitmap is larger than 72x14 pixels.</para>
		/// </exception>
		public static byte[] CreateOperatorLogo(OtaBitmap otaBitmap, string mobileCountryCode, string mobileNetworkCode)
		{
			if (otaBitmap != null)
			{
				if (otaBitmap.Width > 72 || otaBitmap.Height > 14)
				{
					string[] str = new string[5];
					str[0] = "Bitmaps used as operator logos must not be larger than ";
					int num = 72;
					str[1] = num.ToString();
					str[2] = "x";
					int num1 = 14;
					str[3] = num1.ToString();
					str[4] = " pixels.";
					throw new ArgumentException(string.Concat(str));
				}
				else
				{
					if (mobileCountryCode.Length == 3)
					{
						if (mobileNetworkCode.Length == 2)
						{
							byte num2 = 48;
							byte[] numArray = Calc.HexToInt(BcdWorker.EncodeSemiOctets(mobileCountryCode.PadRight(4, 'F')));
							byte[] numArray1 = Calc.HexToInt(BcdWorker.EncodeSemiOctets(mobileNetworkCode.PadRight(2, 'F')));
							int length = 1 + (int)numArray.Length + (int)numArray1.Length + 1;
							int length1 = 0;
							byte[] numArray2 = new byte[length];
							int num3 = length1;
							length1 = num3 + 1;
							numArray2[num3] = num2;
							numArray.CopyTo(numArray2, length1);
							length1 = length1 + (int)numArray.Length;
							numArray1.CopyTo(numArray2, length1);
							length1 = length1 + (int)numArray1.Length;
							int num4 = length1;
							length1 = num4 + 1;
							numArray2[num4] = 10;
							byte[] byteArray = otaBitmap.ToByteArray();
							byte[] numArray3 = new byte[(int)numArray2.Length + (int)byteArray.Length];
							numArray2.CopyTo(numArray3, 0);
							byteArray.CopyTo(numArray3, length1);
							return numArray3;
						}
						else
						{
							throw new ArgumentException("mobileNetworkCode must be 2 digits long.", "mobileNetworkCode");
						}
					}
					else
					{
						throw new ArgumentException("mobileCountryCode must be 3 digits long.", "mobileCountryCode");
					}
				}
			}
			else
			{
				throw new ArgumentNullException("otaBitmap");
			}
		}

		/// <summary>
		/// Creates an operator logo message.
		/// </summary>
		/// <param name="operatorLogo">The operator logo. Use <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageFactory.CreateOperatorLogo(GsmComm.PduConverter.SmartMessaging.OtaBitmap,System.String,System.String)" /> to create one.</param>
		/// <param name="destinationAddress">The message's destination address.</param>
		/// <returns>A set of <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> objects that represent the message.</returns>
		/// <exception cref="T:System.ArgumentNullException">operatorLogo is null.</exception>
		/// <exception cref="T:System.ArgumentException"><para>operatorLogo is an empty array.</para>
		/// <para> -or- </para><para>destinationAddress is an empty string.</para>
		/// <para> -or- </para><para>operatorLogo is so big that it would create more than 255 message parts.</para></exception>
		public static SmsSubmitPdu[] CreateOperatorLogoMessage(byte[] operatorLogo, string destinationAddress)
		{
			int length;
			if (operatorLogo != null)
			{
				if ((int)operatorLogo.Length != 0)
				{
					PortAddressElement16 portAddressElement16 = new PortAddressElement16(5506, 0);
					byte num = (byte)((int)portAddressElement16.ToByteArray().Length);
					int num1 = num + 1;
					bool flag = (int)operatorLogo.Length > 140 - num1;
					if (flag)
					{
						ConcatMessageElement8 concatMessageElement8 = new ConcatMessageElement8(0, 0, 0);
						num = (byte)((int)portAddressElement16.ToByteArray().Length + (int)concatMessageElement8.ToByteArray().Length);
						num1 = num + 1;
					}
					ArrayList arrayLists = new ArrayList();
					int num2 = 0;
					byte num3 = (byte)Math.Ceiling((double)((int)operatorLogo.Length) / (double)(140 - num1));
					if (arrayLists.Count <= 255)
					{
						ushort num4 = 1;
						if (flag)
						{
							num4 = SmartMessageFactory.CalcNextRefNumber();
						}
						for (byte i = 1; i <= num3; i = (byte)(i + 1))
						{
							if ((int)operatorLogo.Length - num2 < 140 - num1)
							{
								length = (int)operatorLogo.Length - num2;
							}
							else
							{
								length = 140 - num1;
							}
							byte[] numArray = new byte[num1];
							int length1 = 0;
							int num5 = length1;
							length1 = num5 + 1;
							numArray[num5] = num;
							portAddressElement16.ToByteArray().CopyTo(numArray, length1);
							length1 = length1 + (int)portAddressElement16.ToByteArray().Length;
							if (flag)
							{
								ConcatMessageElement8 concatMessageElement81 = new ConcatMessageElement8((byte)(num4 % 256), num3, i);
								concatMessageElement81.ToByteArray().CopyTo(numArray, length1);
								length1 = length1 + (int)concatMessageElement81.ToByteArray().Length;
							}
							byte[] numArray1 = new byte[(int)numArray.Length + length];
							numArray.CopyTo(numArray1, 0);
							Array.Copy(operatorLogo, num2, numArray1, length1, length);
							SmsSubmitPdu smsSubmitPdu = new SmsSubmitPdu();
							smsSubmitPdu.MessageFlags.UserDataHeaderPresent = true;
							smsSubmitPdu.DataCodingScheme = 21;
							smsSubmitPdu.DestinationAddress = destinationAddress;
							smsSubmitPdu.SetUserData(numArray1, (byte)((int)numArray1.Length));
							arrayLists.Add(smsSubmitPdu);
							num2 = num2 + length;
						}
						SmsSubmitPdu[] smsSubmitPduArray = new SmsSubmitPdu[arrayLists.Count];
						arrayLists.CopyTo(smsSubmitPduArray, 0);
						return smsSubmitPduArray;
					}
					else
					{
						throw new ArgumentException("A concatenated message must not have more than 255 parts.", "operatorLogo");
					}
				}
				else
				{
					throw new ArgumentException("operatorLogo must not be an empty array.", "operatorLogo");
				}
			}
			else
			{
				throw new ArgumentNullException("operatorLogo");
			}
		}

		/// <summary>
		/// Creates a user data header with port addressing information.
		/// </summary>
		/// <param name="destinationPort">The message's destination port.</param>
		/// <returns>A byte array containing the user data header.</returns>
		public static byte[] CreatePortAddressHeader(ushort destinationPort)
		{
			PortAddressElement16 portAddressElement16 = new PortAddressElement16(destinationPort, 0);
			return SmartMessageFactory.CreateUserDataHeader(portAddressElement16);
		}

		/// <summary>
		/// Creates a user data header out of information elements.
		/// </summary>
		/// <param name="element">The <see cref="T:GsmComm.PduConverter.SmartMessaging.InformationElement" /> instance to be stored in the header.</param>
		/// <returns>A byte array containing the user data header.</returns>
		/// <exception cref="T:System.ArgumentNullException">element is null.</exception>
		/// <exception cref="T:System.ArgumentException">Element is too large, size exceeds 255 bytes.</exception>
		public static byte[] CreateUserDataHeader(InformationElement element)
		{
			if (element != null)
			{
				byte[] byteArray = element.ToByteArray();
				if ((int)byteArray.Length <= 255)
				{
					byte[] length = new byte[(int)byteArray.Length + 1];
					length[0] = (byte)((int)byteArray.Length);
					byteArray.CopyTo(length, 1);
					return length;
				}
				else
				{
					throw new ArgumentException("Element is to large, size exceeds 255 bytes.");
				}
			}
			else
			{
				throw new ArgumentNullException("element");
			}
		}

		/// <summary>
		/// Creates a user data header out of information elements.
		/// </summary>
		/// <param name="elements">The <see cref="T:GsmComm.PduConverter.SmartMessaging.InformationElement" /> instances to be stored in the header.</param>
		/// <returns>A byte array containing the user data header.</returns>
		/// <exception cref="T:System.ArgumentNullException">elements is null.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of all elements is too large, size exceeds 255 bytes.</exception>
		public static byte[] CreateUserDataHeader(InformationElement[] elements)
		{
			if (elements != null)
			{
				List<byte> nums = new List<byte>();
				nums.Add(0);
				int length = 0;
				InformationElement[] informationElementArray = elements;
				for (int i = 0; i < (int)informationElementArray.Length; i++)
				{
					InformationElement informationElement = informationElementArray[i];
					byte[] byteArray = informationElement.ToByteArray();
					nums.AddRange(byteArray);
					length = length + (int)byteArray.Length;
				}
				if (length <= 255)
				{
					nums[0] = (byte)length;
					return nums.ToArray();
				}
				else
				{
					throw new ArgumentException("The sum of all elements is too large, size exceeds 255 bytes.");
				}
			}
			else
			{
				throw new ArgumentNullException("elements");
			}
		}
	}
}