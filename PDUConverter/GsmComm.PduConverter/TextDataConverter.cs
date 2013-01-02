using System;
using System.Collections;
using System.Text;

/// <summary>
/// Converts between text strings and PDU-compatible formats.
/// </summary>
namespace GsmComm.PduConverter
{
	public static class TextDataConverter
	{
		/// <summary>
		/// The GSM 7-Bit default alphabet.
		/// </summary>
		private static char[] sevenBitDefault;

		static TextDataConverter()
		{
			char[] chrArray = new char[] { '@', '£', '$', '¥', 'è', 'é', 'ù', 'ì', 'ò', 'Ç', '\n', 'Ø', 'ø', '\r', 'Å', 'å', 'Δ', '\u005F', 'Φ', 'Γ', 'Λ', 'Ω', 'Π', 'Ψ', 'Σ', 'Θ', 'Ξ', '\u001B', 'Æ', 'æ', 'ß', 'É', ' ', '!', '\"', '#', '¤', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', '¡', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'Ä', 'Ö', 'Ñ', 'Ü', '\u00A7', '¿', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'ä', 'ö', 'ñ', 'ü', 'à' };
			TextDataConverter.sevenBitDefault = chrArray;
		}

		private static byte CharToSevenBitExtension(char c)
		{
			char chr = c;
			if (chr > '\u005E')
			{
				switch (chr)
				{
					case '{':
					{
						return 40;
					}
					case '|':
					{
						return 64;
					}
					case '}':
					{
						return 41;
					}
					case '~':
					{
						return 61;
					}
					default:
					{
						if (chr == '€')
						{
							return 101;
						}
						else
						{
							break;
						}
					}
				}
			}
			else
			{
				if (chr == '\f')
				{
					return 10;
				}
				else
				{
					switch (chr)
					{
						case '[':
						{
							return 60;
						}
						case '\\':
						{
							return 47;
						}
						case ']':
						{
							return 62;
						}
						case '\u005E':
						{
							return 20;
						}
					}
				}
			}
			throw new ArgumentException(string.Concat("The character '", c.ToString(), "' does not exist in the GSM 7-bit default alphabet extension table."));
		}

		/// <summary>
		/// Expands an array of octets into string of septets.
		/// </summary>
		/// <param name="data">An array of 8-bit encoded data (octets), represented as a string.</param>
		/// <returns>The converted data as a string.</returns>
		public static string OctetsToSeptetsStr(byte[] data)
		{
			string str;
			if (data != null)
			{
				string empty = string.Empty;
				string empty1 = string.Empty;
                int i = 0;
				for (; i < (int)data.Length; i++)
				{
					string bin = Calc.IntToBin(data[i], 8);
					string str1 = bin.Substring(i % 7 + 1, 7 - i % 7);
					if (i == 0 || i % 7 == 0)
					{
						if (i != 0)
						{
							empty = string.Concat(empty, (char)Calc.BinToInt(empty1));
						}
						str = str1;
					}
					else
					{
						str = string.Concat(str1, empty1);
					}
					empty1 = bin.Substring(0, i % 7 + 1);
					empty = string.Concat(empty, (char)Calc.BinToInt(str));
				}
				if (i != 0 && i % 7 == 0 && empty1 != "0000000")
				{
					empty = string.Concat(empty, (char)Calc.BinToInt(empty1));
				}
				return empty;
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Compacts a string of septets into octets.
		/// </summary>
		/// <param name="data">7-bit encoded data (septets), represented as a string.</param>
		/// <remarks>
		/// <par>When only 7 of 8 available bits of a character are used, 1 bit is
		/// wasted per character. This method compacts a string of characters
		/// which consist solely of such 7-bit characters.</par>
		/// <par>Effectively, every 8 bytes of the original string are packed into
		/// 7 bytes in the resulting string.</par>
		/// </remarks>
		public static string SeptetsToOctetsHex(string data)
		{
			string empty = string.Empty;
			string str = string.Empty;
			for (int i = 0; i < data.Length; i++)
			{
				string bin = Calc.IntToBin((byte)data[i], 7);
				if (i != 0 && i % 8 != 0)
				{
					string str1 = bin.Substring(7 - i % 8);
					string str2 = string.Concat(str1, str);
					empty = string.Concat(empty, Calc.IntToHex(Calc.BinToInt(str2)));
				}
				str = bin.Substring(0, 7 - i % 8);
				if (i == data.Length - 1 && str != string.Empty)
				{
					empty = string.Concat(empty, Calc.IntToHex(Calc.BinToInt(str)));
				}
			}
			return empty;
		}

		/// <summary>
		/// Compacts a string of septets into octets.
		/// </summary>
		/// <param name="data">7-bit encoded data (septets), represented as a string.</param>
		/// <remarks>
		/// <par>When only 7 of 8 available bits of a character are used, 1 bit is
		/// wasted per character. This method compacts a string of characters
		/// which consist solely of such 7-bit characters.</par>
		/// <par>Effectively, every 8 bytes of the original string are packed into
		/// 7 bytes in the resulting string.</par>
		/// </remarks>
		public static byte[] SeptetsToOctetsInt(string data)
		{
			ArrayList arrayLists = new ArrayList();
			string empty = string.Empty;
			for (int i = 0; i < data.Length; i++)
			{
				string bin = Calc.IntToBin((byte)data[i], 7);
				if (i != 0 && i % 8 != 0)
				{
					string str = bin.Substring(7 - i % 8);
					string str1 = string.Concat(str, empty);
					arrayLists.Add(Calc.BinToInt(str1));
				}
				empty = bin.Substring(0, 7 - i % 8);
				if (i == data.Length - 1 && empty != string.Empty)
				{
					arrayLists.Add(Calc.BinToInt(empty));
				}
			}
			byte[] numArray = new byte[arrayLists.Count];
			arrayLists.CopyTo(numArray);
			return numArray;
		}

		private static char SevenBitExtensionToChar(byte b)
		{
			byte num = b;
			if (num > 41)
			{
				if (num == 47)
				{
					return '\\';
				}
				else
				{
					if (num == 60)
					{
						return '[';
					}
					else if (num == 61)
					{
						return '~';
					}
					else if (num == 62)
					{
						return ']';
					}
					else if (num == 63)
					{
						throw new ArgumentException(string.Concat("The value ", b.ToString(), " is not part of the 7-bit default alphabet extension table."));
					}
					else if (num == 64)
					{
						return '|';
					}
					if (num == 101)
					{
						return '€';
					}
				}
			}
			else
			{
				if (num == 10)
				{
					return '\f';
				}
				else
				{
					if (num == 20)
					{
						return '\u005E';
					}
					else
					{
						switch (num)
						{
							case 40:
							{
								return '{';
							}
							case 41:
							{
								return '}';
							}
						}
					}
				}
			}
			throw new ArgumentException(string.Concat("The value ", b.ToString(), " is not part of the 7-bit default alphabet extension table."));
		}

		/// <summary>
		/// Converts a string consisting of characters from the GSM
		/// "7-bit default alphabet" into a string of corresponding characters
		/// of the ISO-8859-1 character set.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>The converted string.</returns>
		/// <remarks>
		/// <para>Note that the converted string does not necessarily have the same
		/// length as the original one because some characters may be escaped.</para>
		/// <para>This method throws an exception if an invalid character
		/// is encountered.</para>
		/// </remarks>
		public static string SevenBitToString(string s)
		{
			return TextDataConverter.SevenBitToString(s, true);
		}

		/// <summary>
		/// Converts a string consisting of characters from the GSM
		/// "7-bit default alphabet" into a string of corresponding characters
		/// of the ISO-8859-1 character set.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <param name="throwExceptions">If true, throws an exception if
		/// an invalid character is encountered. If false, replaces every
		/// unknown character with a question mark (?).</param>
		/// <returns>The converted string.</returns>
		/// <remarks>
		/// <para>Note that the converted string does not necessarily have the same
		/// length as the original one because some characters may be escaped.</para>
		/// <para>This method throws an exception if an invalid character
		/// is encountered.</para>
		/// </remarks>
		public static string SevenBitToString(string s, bool throwExceptions)
		{
			string empty = string.Empty;
			bool flag = false;
			for (int i = 0; i < s.Length; i++)
			{
				byte num = (byte)s[i];
				if (!flag)
				{
					if (num == 27)
					{
						flag = true;
					}
					else
					{
						if (num > TextDataConverter.sevenBitDefault.GetUpperBound(0))
						{
							if (!throwExceptions)
							{
								empty = string.Concat(empty, (char)63);
							}
							else
							{
								object[] str = new object[5];
								str[0] = "Character '";
								str[1] = (char)num;
								str[2] = "' at position ";
								int num1 = i + 1;
								str[3] = num1.ToString();
								str[4] = " is not part of the GSM 7-bit default alphabet.";
								throw new ArgumentException(string.Concat(str));
							}
						}
						else
						{
							empty = string.Concat(empty, TextDataConverter.sevenBitDefault[num]);
						}
					}
				}
				else
				{
					try
					{
						empty = string.Concat(empty, TextDataConverter.SevenBitExtensionToChar(num));
					}
					catch (Exception exception)
					{
						if (!throwExceptions)
						{
							empty = string.Concat(empty, (char)63);
						}
						else
						{
							throw;
						}
					}
					flag = false;
				}
			}
			return empty;
		}

		/// <summary>
		/// Converts a string consisting of characters from the ISO-8859-1
		/// character set into a string of corresponding characters of the
		/// "GSM 7-bit default alphabet" character set.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <param name="throwExceptions">If true, throws an exception if
		/// an invalid character is encountered. If false, replaces every
		/// unknown character with a question mark (?).</param>
		/// <param name="charsReplaced">Will be set to true if invalid characters
		/// were replaced. <b>throwExceptions</b> must be false for this to work.</param>
		/// <returns>The converted string.</returns>
		/// <remarks>
		/// Note that the converted string does not need to have the same
		/// length as the original one because some characters may be escaped.
		/// </remarks>
		/// <exception cref="T:System.ArgumentException">throwExceptions is true and invalid character is encountered in the string.</exception>
		public static string StringTo7Bit(string s, bool throwExceptions, out bool charsReplaced)
		{
			StringBuilder stringBuilder = new StringBuilder();
			charsReplaced = false;
			bool flag = false;
			int i = 0;
			int num = 0;
			for (i = 0; i < s.Length; i++)
			{
				flag = false;
				char chr = s.Substring(i, 1)[0];
				num = 0;
				while (num <= TextDataConverter.sevenBitDefault.GetUpperBound(0))
				{
					if (TextDataConverter.sevenBitDefault[num] != chr)
					{
						num++;
					}
					else
					{
						stringBuilder.Append((ushort)num);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					try
					{
						byte sevenBitExtension = TextDataConverter.CharToSevenBitExtension(chr);
						stringBuilder.Append('\u001B');
						stringBuilder.Append(sevenBitExtension);
						flag = true;
					}
					catch (Exception exception)
					{
					}
				}
				if (!flag)
				{
					if (!throwExceptions)
					{
						stringBuilder.Append('?');
						charsReplaced = true;
					}
					else
					{
						object[] str = new object[5];
						str[0] = "The character '";
						str[1] = chr;
						str[2] = "' at position ";
						int num1 = i + 1;
						str[3] = num1.ToString();
						str[4] = " does not exist in the GSM 7-bit default alphabet.";
						throw new ArgumentException(string.Concat(str));
					}
				}
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Converts a string consisting of characters from the ISO-8859-1
		/// character set into a string of corresponding characters of the
		/// "GSM 7-bit default alphabet" character set.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>The converted string.</returns>
		/// <remarks>
		/// <para>Throws an exception when an invalid character is encountered in the string.</para>
		/// <para>Note that the converted string does not need to have the same
		/// length as the original one because some characters may be escaped.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentException">An invalid character is encountered in the string.</exception>
		public static string StringTo7Bit(string s)
		{
			bool flag = false;
			return TextDataConverter.StringTo7Bit(s, true, out flag);
		}
	}
}