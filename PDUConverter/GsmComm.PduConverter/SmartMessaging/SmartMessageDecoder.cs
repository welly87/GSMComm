using GsmComm.PduConverter;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Decodes messages based on Nokia's Smart Messaging specification and related messages.
/// </summary>
/// <remarks>
/// <para>This methods in this class can be used to find and combine concatenated (long, multi-part) SMS messages.</para>
/// <para>To determine, whether a message is part of a concatenated message at all, use <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.IsPartOfConcatMessage(GsmComm.PduConverter.SmsPdu)" />
/// When you have identified that a message is a part of a concatenated message, you need to find the other message parts
/// belonging to the same message, <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.ArePartOfSameMessage(GsmComm.PduConverter.SmsPdu,GsmComm.PduConverter.SmsPdu)" /> can be used for this.
/// </para>
/// <para>After all parts of the concatenated message have been identified, the parts need to be combined. This is done
/// using <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.CombineConcatMessage(System.Collections.Generic.IList{GsmComm.PduConverter.SmsPdu})" /> and <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.CombineConcatMessageText(System.Collections.Generic.IList{GsmComm.PduConverter.SmsPdu})" />. The difference between these two
/// methods is, that the first one returns the combined data in binary format, whereas the latter returns the result
/// as text.</para>
/// <para>To verify that all message parts are present before attempting to combine them, <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.AreAllConcatPartsPresent(System.Collections.Generic.IList{GsmComm.PduConverter.SmsPdu})" />
/// can be used. Calling this method is optional, but prevents exceptions when combining without all parts present.</para>
/// <para>The above methods all accept <see cref="T:GsmComm.PduConverter.SmsPdu" /> instances to abstract some of the work that has to be done
/// to find and combine concatenated SMS messages. However, it is also possible to retrieve the underlying data that
/// is used for these operations (and also possible for other operations as well) using the methods <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.DecodeUserDataHeader(System.Byte[])" />
/// and <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.GetConcatenationInfo(GsmComm.PduConverter.SmsPdu)" />.</para>
/// </remarks>
namespace GsmComm.PduConverter.SmartMessaging
{
	public class SmartMessageDecoder
	{
		public SmartMessageDecoder()
		{
		}

		/// <summary>
		/// Determines whether all parts of a concatenated message are present.
		/// </summary>
		/// <param name="parts">The parts that make up the concatenated message.</param>
		/// <returns>true if all parts are present, false otherwise.</returns>
		/// <exception cref="T:System.ArgumentNullException">parts is null.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <para>The reference numbers differ between the message parts.</para>
		/// <para> -or- </para>
		/// <para>The number of total messages differs between the message parts.</para>
		/// <para> -or- </para>
		/// <para>A non-concatenated message part is present at an invalid position.</para>
		/// </exception>
		public static bool AreAllConcatPartsPresent(IList<SmsPdu> parts)
		{
			bool flag = false;
			SmartMessageDecoder.GetConcatUserData(parts, false, true, true, out flag);
			return flag;
		}

		/// <summary>
		/// Determines whether two messages are part of the same concatenated message.
		/// </summary>
		/// <param name="pdu1">The first message to compare.</param>
		/// <param name="pdu2">The second message to compare.</param>
		/// <returns>true if both messages appear to belong to the same concatenated message, false otherwise.</returns>
		/// <remarks>
		/// <para>This comparison is supported for <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> and <see cref="T:GsmComm.PduConverter.SmsDeliverPdu" /> objects.
		/// For all other objects, this comparison always returns false.</para>
		/// <para>For <see cref="T:GsmComm.PduConverter.SmsSubmitPdu" /> objects, the <see cref="P:GsmComm.PduConverter.SmsSubmitPdu.DestinationAddress" />,
		/// <see cref="P:GsmComm.PduConverter.SmsSubmitPdu.DestinationAddressType" /> and <see cref="P:GsmComm.PduConverter.SmartMessaging.IConcatenationInfo.ReferenceNumber" /> properties are compared.</para>
		/// <para>For <see cref="T:GsmComm.PduConverter.SmsDeliverPdu" /> objects, the <see cref="P:GsmComm.PduConverter.SmsDeliverPdu.OriginatingAddress" />,
		/// <see cref="P:GsmComm.PduConverter.SmsDeliverPdu.OriginatingAddressType" /> and <see cref="P:GsmComm.PduConverter.SmartMessaging.IConcatenationInfo.ReferenceNumber" /> properties are compared.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException">pdu1 or pdu2 is null.</exception>
		public static bool ArePartOfSameMessage(SmsPdu pdu1, SmsPdu pdu2)
		{
			if (pdu1 != null)
			{
				if (pdu2 != null)
				{
					bool flag = false;
					if (pdu1 as SmsDeliverPdu == null || pdu2 as SmsDeliverPdu == null)
					{
						if (pdu1 is SmsSubmitPdu && pdu2 is SmsSubmitPdu)
						{
							SmsSubmitPdu smsSubmitPdu = (SmsSubmitPdu)pdu1;
							SmsSubmitPdu smsSubmitPdu1 = (SmsSubmitPdu)pdu2;
							if (smsSubmitPdu.DestinationAddress == smsSubmitPdu1.DestinationAddress && smsSubmitPdu.DestinationAddressType == smsSubmitPdu1.DestinationAddressType)
							{
								flag = SmartMessageDecoder.HaveSameReferenceNumber(pdu1, pdu2);
							}
						}
					}
					else
					{
						SmsDeliverPdu smsDeliverPdu = (SmsDeliverPdu)pdu1;
						SmsDeliverPdu smsDeliverPdu1 = (SmsDeliverPdu)pdu2;
						if (smsDeliverPdu.OriginatingAddress == smsDeliverPdu1.OriginatingAddress && smsDeliverPdu.OriginatingAddressType == smsDeliverPdu1.OriginatingAddressType)
						{
							flag = SmartMessageDecoder.HaveSameReferenceNumber(pdu1, pdu2);
						}
					}
					return flag;
				}
				else
				{
					throw new ArgumentNullException("pdu2");
				}
			}
			else
			{
				throw new ArgumentNullException("pdu1");
			}
		}

		/// <summary>
		/// Combines the parts of a concatenated message into a single message.
		/// </summary>
		/// <param name="parts">The parts that make up the concatenated message.</param>
		/// <returns>A byte array containing the combined user data of all parts without any headers.</returns>
		/// <remarks>
		/// <para>All parts must be available, but can be in any order.</para>
		/// <para>The user data is returned in its binary format. If you want the user data to be
		/// returned as text, use <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.CombineConcatMessageText(System.Collections.Generic.IList{GsmComm.PduConverter.SmsPdu})" /> instead.</para>
		/// <para>If the first part is a non-concatenated message, its user data is returned back, and no more parts are processed
		/// afterwards.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException">parts is null.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <para>Not all parts of the message are available.</para>
		/// <para> -or- </para>
		/// <para>The reference numbers differ between the message parts.</para>
		/// <para> -or- </para>
		/// <para>The number of total messages differs between the message parts.</para>
		/// <para> -or- </para>
		/// <para>A non-concatenated message part is present at an invalid position.</para>
		/// </exception>
		/// <seealso cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.CombineConcatMessageText(System.Collections.Generic.IList{GsmComm.PduConverter.SmsPdu})" />
		public static byte[] CombineConcatMessage(IList<SmsPdu> parts)
		{
			bool flag = false;
			List<object> concatUserData = SmartMessageDecoder.GetConcatUserData(parts, false, false, false, out flag);
			List<byte> nums = new List<byte>();
			foreach (byte[] concatUserDatum in concatUserData)
			{
				nums.AddRange(concatUserDatum);
			}
			return nums.ToArray();
		}

		/// <summary>
		/// Combines the parts of a concatenated message into a single message text.
		/// </summary>
		/// <param name="parts">The parts that make up the concatenated message.</param>
		/// <returns>A string containing the combined message text of all parts.</returns>
		/// <remarks>
		/// <para>All parts must be available, but can be in any order.</para>
		/// <para>The user data is converted into text according to the data coding scheme specified in the message.
		/// If you want the user data to be returned in its binary format, use <see cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.CombineConcatMessage(System.Collections.Generic.IList{GsmComm.PduConverter.SmsPdu})" /> instead.</para>
		/// <para>If the first part is a non-concatenated message, its user data is returned back as text, and no more parts are processed
		/// afterwards.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException">parts is null.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <para>Not all parts of the message are available.</para>
		/// <para> -or- </para>
		/// <para>The reference numbers differ between the message parts.</para>
		/// <para> -or- </para>
		/// <para>The number of total messages differs between the message parts.</para>
		/// <para> -or- </para>
		/// <para>A non-concatenated message part is present at an invalid position.</para>
		/// </exception>
		/// <seealso cref="M:GsmComm.PduConverter.SmartMessaging.SmartMessageDecoder.CombineConcatMessage(System.Collections.Generic.IList{GsmComm.PduConverter.SmsPdu})" />
		public static string CombineConcatMessageText(IList<SmsPdu> parts)
		{
			bool flag = false;
			List<object> concatUserData = SmartMessageDecoder.GetConcatUserData(parts, true, false, false, out flag);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string concatUserDatum in concatUserData)
			{
				stringBuilder.Append(concatUserDatum);
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Decodes a user data header into information elements.
		/// </summary>
		/// <param name="userDataHeader">The user data header to be decoded.</param>
		/// <returns>The elements found as an array of <see cref="T:GsmComm.PduConverter.SmartMessaging.InformationElement" /> objects.</returns>
		/// <remarks>
		/// <para>Known information elements are decoded into their respective objects, while unknown
		/// information elements are stored in generic <see cref="T:GsmComm.PduConverter.SmartMessaging.UnknownInformationElement" /> objects.</para>
		/// <para>The list of known information elements consists of elements used within Smart Messaging,
		/// and does not aim to be a complete set of all existing elements.</para>
		/// <para>The currently recognized elements are:</para>
		/// <list type="bullet">
		/// <item><description><see cref="T:GsmComm.PduConverter.SmartMessaging.ConcatMessageElement8" /></description></item>
		/// <item><description><see cref="T:GsmComm.PduConverter.SmartMessaging.ConcatMessageElement16" /></description></item>
		/// <item><description><see cref="T:GsmComm.PduConverter.SmartMessaging.PortAddressElement16" /></description></item>
		/// </list>
		/// </remarks>
		public static InformationElement[] DecodeUserDataHeader(byte[] userDataHeader)
		{
			InformationElement unknownInformationElement;
			List<InformationElement> informationElements = new List<InformationElement>();
			byte num = 0;
			byte num1 = userDataHeader[0];
			if (num1 > 0)
			{
				num = (byte)(num + 1);
				do
				{
					byte num2 = num;
					num = (byte)(num2 + 1);
					byte num3 = userDataHeader[num2];
					byte num4 = num;
					num = (byte)(num4 + 1);
					byte num5 = userDataHeader[num4];
					byte[] numArray = new byte[num5 + 2];
					Array.Copy(userDataHeader, num - 2, numArray, 0, num5 + 2);
					num = (byte)(num + num5);
					if (num3 != 0)
					{
						if (num3 != 8)
						{
							if (num3 != 5)
							{
								unknownInformationElement = new UnknownInformationElement(numArray);
							}
							else
							{
								unknownInformationElement = new PortAddressElement16(numArray);
							}
						}
						else
						{
							unknownInformationElement = new ConcatMessageElement16(numArray);
						}
					}
					else
					{
						unknownInformationElement = new ConcatMessageElement8(numArray);
					}
					informationElements.Add(unknownInformationElement);
				}
				while (num < num1);
			}
			return informationElements.ToArray();
		}

		/// <summary>
		/// Gets the concatenation information of a message.
		/// </summary>
		/// <param name="pdu">The message to get the information of.</param>
		/// <returns>An object implementing <see cref="T:GsmComm.PduConverter.SmartMessaging.IConcatenationInfo" />, if
		/// the message is a part of a concatenated message, null otherwise.</returns>
		/// <remarks>
		/// <para>The returned information can be used to discover the parts of a concatenated message
		/// or recombine the parts back into one message.</para>
		/// </remarks>
		public static IConcatenationInfo GetConcatenationInfo(SmsPdu pdu)
		{
			if (pdu != null)
			{
				IConcatenationInfo concatenationInfo = null;
				if (pdu.UserDataHeaderPresent)
				{
					byte[] userDataHeader = pdu.GetUserDataHeader();
					InformationElement[] informationElementArray = SmartMessageDecoder.DecodeUserDataHeader(userDataHeader);
					InformationElement[] informationElementArray1 = informationElementArray;
					int num = 0;
					while (num < (int)informationElementArray1.Length)
					{
						InformationElement informationElement = informationElementArray1[num];
						if (informationElement as IConcatenationInfo == null)
						{
							num++;
						}
						else
						{
							concatenationInfo = (IConcatenationInfo)informationElement;
							break;
						}
					}
				}
				return concatenationInfo;
			}
			else
			{
				throw new ArgumentNullException("pdu");
			}
		}

		/// <summary>
		/// Retrieves the user data of all parts of a concatenated message.
		/// </summary>
		/// <param name="parts">The parts that make up the concatenated message.</param>
		/// <param name="outputAsText">If true, formats the returned user data as text. If false, returns the user data
		/// in its binary form.</param>
		/// <param name="allowMissingParts">Specifies whether missing parts are allowed. If true, null is returned
		/// in the resulting list in place of every missing part. If false, an exception is raised when a part is
		/// missing.</param>
		/// <param name="noOutput">If set to true, does not fill the returned list with data. If set to false, the data is returned
		/// normally. Use this in conjunction with allowMissingParts set to true to verify whether all message parts are present.</param>
		/// <param name="allPartsAvailable">Is set to true if all message parts are available, false otherwise. Use this in conjunction
		/// with allowMissingParts set to true to verify whether all message parts are present.</param>
		/// <returns>A list of objects containing the user data of every part without any headers.
		/// The outputAsText parameter determines the actual data type that is returned. If outputAsText is true, the return type is a
		/// list of byte arrays, if false a list of strings is returned.
		/// </returns>
		/// <remarks>
		/// <para>The parts can be in any order.</para>
		/// <para>If the first part is a non-concatenated message, its user data is returned back, and no more parts are processed
		/// afterwards.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException">parts is null.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <para>Not all parts of the message are available and allowMissingParts is false.</para>
		/// <para> -or- </para>
		/// <para>The reference numbers differ between the message parts.</para>
		/// <para> -or- </para>
		/// <para>The number of total messages differs between the message parts.</para>
		/// <para> -or- </para>
		/// <para>A non-concatenated message part is present at an invalid position.</para>
		/// </exception>
		private static List<object> GetConcatUserData(IList<SmsPdu> parts, bool outputAsText, bool allowMissingParts, bool noOutput, out bool allPartsAvailable)
		{
			if (parts != null)
			{
				allPartsAvailable = true;
				List<object> objs = new List<object>();
				if (parts.Count > 0)
				{
					SmsPdu item = parts[0];
					if (!SmartMessageDecoder.IsPartOfConcatMessage(item))
					{
						if (!noOutput)
						{
							if (!outputAsText)
							{
								objs.Add(item.UserData);
							}
							else
							{
								objs.Add(item.UserDataText);
							}
						}
					}
					else
					{
						SortedList<IConcatenationInfo, SmsPdu> concatenationInfos = SmartMessageDecoder.SortConcatMessageParts(parts);
						int totalMessages = concatenationInfos.Keys[0].TotalMessages;
						int num = 0;
						for (int i = 1; i <= totalMessages; i++)
						{
							bool flag = false;
							if (num < concatenationInfos.Count)
							{
								IConcatenationInfo concatenationInfo = concatenationInfos.Keys[num];
								SmsPdu smsPdu = concatenationInfos.Values[num];
								if (i == concatenationInfo.CurrentNumber)
								{
									if (!noOutput)
									{
										if (!outputAsText)
										{
											objs.Add(smsPdu.GetUserDataWithoutHeader());
										}
										else
										{
											objs.Add(smsPdu.GetUserDataTextWithoutHeader());
										}
									}
									num++;
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								flag = true;
							}
							if (flag)
							{
								allPartsAvailable = false;
								if (!allowMissingParts)
								{
									throw new ArgumentException(string.Concat("Not all parts of the message are available. Part #", i, " is missing."), "parts");
								}
								else
								{
									if (!noOutput)
									{
										objs.Add(null);
									}
								}
							}
						}
					}
				}
				return objs;
			}
			else
			{
				throw new ArgumentNullException("parts");
			}
		}

		private static bool HaveSameReferenceNumber(SmsPdu pdu1, SmsPdu pdu2)
		{
			bool flag = false;
			if (SmartMessageDecoder.IsPartOfConcatMessage(pdu1) && SmartMessageDecoder.IsPartOfConcatMessage(pdu2))
			{
				IConcatenationInfo concatenationInfo = SmartMessageDecoder.GetConcatenationInfo(pdu1);
				IConcatenationInfo concatenationInfo1 = SmartMessageDecoder.GetConcatenationInfo(pdu2);
				if (concatenationInfo.ReferenceNumber == concatenationInfo1.ReferenceNumber)
				{
					flag = true;
				}
			}
			return flag;
		}

		/// <summary>
		/// Determines whether a message is part of a concatenated message.
		/// </summary>
		/// <param name="pdu">The message.</param>
		/// <returns>true if the message is part of a concatenated message, false otherwise.</returns>
		public static bool IsPartOfConcatMessage(SmsPdu pdu)
		{
			return SmartMessageDecoder.GetConcatenationInfo(pdu) != null;
		}

		private static SortedList<IConcatenationInfo, SmsPdu> SortConcatMessageParts(IList<SmsPdu> parts)
		{
			IConcatenationInfo concatenationInfo;
			IConcatenationInfo concatenationInfo1 = null;
			SortedList<IConcatenationInfo, SmsPdu> concatenationInfos = new SortedList<IConcatenationInfo, SmsPdu>(new ConcatInfoComparer());
			foreach (SmsPdu part in parts)
			{
				if (concatenationInfo1 != null)
				{
					if (!SmartMessageDecoder.IsPartOfConcatMessage(part))
					{
						throw new ArgumentException("A non-concatenated message part is present at an invalid position.", "parts");
					}
					else
					{
						concatenationInfo = SmartMessageDecoder.GetConcatenationInfo(part);
						if (concatenationInfo1.ReferenceNumber == concatenationInfo.ReferenceNumber)
						{
							if (concatenationInfo1.TotalMessages != concatenationInfo.TotalMessages)
							{
								throw new ArgumentException("The number of total messages differs between the message parts.", "parts");
							}
						}
						else
						{
							throw new ArgumentException("The reference numbers differ between the message parts.", "parts");
						}
					}
				}
				else
				{
					concatenationInfo = SmartMessageDecoder.GetConcatenationInfo(part);
				}
				concatenationInfos.Add(concatenationInfo, part);
				concatenationInfo1 = concatenationInfo;
			}
			return concatenationInfos;
		}
	}
}