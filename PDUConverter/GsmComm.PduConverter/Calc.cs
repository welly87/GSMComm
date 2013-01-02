using System;

/// <summary>
/// Performs various numerical conversions and calculations.
/// </summary>
namespace GsmComm.PduConverter
{
	public class Calc
	{
		private static char[] hexDigits;

		static Calc()
		{
			char[] chrArray = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
			Calc.hexDigits = chrArray;
		}

		public Calc()
		{
		}

		/// <summary>
		/// Converts a bit string into a byte.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>The converted value.</returns>
		public static byte BinToInt(string s)
		{
			return Convert.ToByte(s, 2);
		}

		/// <summary>
		/// Converts a BCD encoded string (hexadecimal) into its byte representation.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <remarks>
		/// The length of the string should be even. This is not checked
		/// here to be able to process truncated strings.
		/// </remarks>
		/// <returns>The converted value.</returns>
		/// <example>
		/// <para>A string containing "41" will become {0x41}, which equals
		/// the character 'A'.</para>
		/// <para>A string containing "414242" will become {0x41, 0x42, 0x43}
		/// which equals the string "ABC".</para>
		/// </example>
		public static byte[] HexToInt(string s)
		{
			byte[] num = new byte[s.Length / 2];
			for (int i = 0; i < s.Length / 2; i++)
			{
				string str = s.Substring(i * 2, 2);
				num[i] = Convert.ToByte(str, 16);
			}
			return num;
		}

		/// <summary>
		/// Converts a byte into a bit string.
		/// </summary>
		/// <param name="b">The byte to convert.</param>
		/// <param name="size">
		/// The final length the string should have. If the resulting string is
		/// shorter than this value, it is padded with leading zeroes.
		/// </param>
		/// <returns>The converted value.</returns>
		public static string IntToBin(byte b, byte size)
		{
			return Convert.ToString(b, 2).PadLeft(size, '0');
		}

		/// <summary>
		/// Converts a byte array into its hexadecimal representation (BCD encoding).
		/// </summary>
		/// <param name="bytes">The byte array to convert.</param>
		/// <returns>The converted value.</returns>
		public static string IntToHex(byte[] bytes)
		{
			char[] chrArray = new char[(int)bytes.Length * 2];
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				int num = bytes[i];
				chrArray[i * 2] = Calc.hexDigits[num >> 4];
				chrArray[i * 2 + 1] = Calc.hexDigits[num & 15];
			}
			return new string(chrArray);
		}

		/// <summary>
		/// Converts a byte array into its hexadecimal representation (BCD encoding).
		/// </summary>
		/// <param name="bytes">The byte array to convert.</param>
		/// <param name="index">The starting index of the byte array to convert.</param>
		/// <param name="count">The number of bytes to convert.</param>
		/// <returns>The converted value.</returns>
		public static string IntToHex(byte[] bytes, int index, int count)
		{
			char[] chrArray = new char[count * 2];
			for (int i = 0; i < count; i++)
			{
				int num = bytes[index + i];
				chrArray[i * 2] = Calc.hexDigits[num >> 4];
				chrArray[i * 2 + 1] = Calc.hexDigits[num & 15];
			}
			return new string(chrArray);
		}

		/// <summary>
		/// Converts a byte into its BCD (hexadecimal) representation.
		/// </summary>
		/// <param name="b">The byte to convert.</param>
		/// <returns>The converted value.</returns>
		public static string IntToHex(byte b)
		{
			return string.Concat(Calc.hexDigits[b >> 4].ToString(), Calc.hexDigits[b & 15].ToString());
		}

		/// <summary>
		/// Determines if a string is a hexadecimal character.
		/// </summary>
		/// <param name="c">The character to check.</param>
		/// <returns>true if the character is a hex char, false otherwise.</returns>
		public static bool IsHexDigit(char c)
		{
			char upper = char.ToUpper(c);
			char[] chrArray = Calc.hexDigits;
			int num = 0;
			while (num < (int)chrArray.Length)
			{
				char chr = chrArray[num];
				if (upper != chr)
				{
					num++;
				}
				else
				{
					bool flag = true;
					return flag;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines if a string consists only of hexadecimal characters.
		/// </summary>
		/// <param name="s">The string to check.</param>
		/// <returns>true if the string is a hex string, false otherwise.</returns>
		public static bool IsHexString(string s)
		{
			if (s.Length != 0)
			{
				int num = 0;
				while (num < s.Length)
				{
					if (Calc.IsHexDigit(s[num]))
					{
						num++;
					}
					else
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}