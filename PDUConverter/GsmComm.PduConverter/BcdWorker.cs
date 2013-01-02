using System;

/// <summary>
/// A class for working with BCD encoded data strings.
/// </summary>
namespace GsmComm.PduConverter
{
	public class BcdWorker
	{
		public BcdWorker()
		{
		}

		/// <summary>
		/// Counts the number of bytes in a BCD encoded string.
		/// </summary>
		/// <param name="s">The string containing the BCD data.</param>
		/// <returns>The byte count.</returns>
		public static int CountBytes(string s)
		{
			return s.Length / 2;
		}

		/// <summary>
		/// Swaps the semi-octets of a BCD encoded string and checks the length.
		/// </summary>
		/// <param name="data">The string to decode. Must be of even length.</param>
		/// <returns>The converted value.</returns>
		/// <exception cref="T:System.ArgumentException">String length is not even.</exception>
		/// <example>21436587 becomes 12345678.</example>
		public static string DecodeSemiOctets(string data)
		{
			if (data.Length % 2 == 0)
			{
				string empty = string.Empty;
				for (int i = 0; i < data.Length; i = i + 2)
				{
					empty = string.Concat(empty, data.Substring(i + 1, 1), data.Substring(i, 1));
				}
				return empty;
			}
			else
			{
				throw new ArgumentException("String length must be even.");
			}
		}

		/// <summary>
		/// Swaps the semi-octets of a BCD encoded string.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <remarks>
		/// <para>If the string is not of even length, it is padded with a
		/// hexadecimal "F" before converting.</para>
		/// <para>This method does not verify the actual contents of the string.</para>
		/// </remarks>
		/// <returns>The converted value.</returns>
		/// <example>
		/// <param>A string containing "12345678" will become "21436587".</param>
		/// <param>A string containing "1234567" will become "214365F7".</param>
		/// </example>
		public static string EncodeSemiOctets(string data)
		{
			if (data.Length % 2 != 0)
			{
				data = string.Concat(data, "F");
			}
			string empty = string.Empty;
			for (int i = 0; i < data.Length; i = i + 2)
			{
				empty = string.Concat(empty, data.Substring(i + 1, 1), data.Substring(i, 1));
			}
			return empty;
		}

		/// <summary>
		/// Swaps the semi-octets of a BCD encoded string.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <param name="totalWidth">The width to pad the string to before converting.
		/// Padding character is hexadecimal "F".</param>
		/// <remarks>
		/// <para>This method does not verify the actual contents of the string.</para>
		/// </remarks>
		/// <returns>The converted value.</returns>
		/// <exception cref="T:System.ArgumentException">totalWidth is not even.</exception>
		public static string EncodeSemiOctets(string data, int totalWidth)
		{
			if (totalWidth % 2 == 0)
			{
				return BcdWorker.EncodeSemiOctets(data.PadRight(totalWidth, 'F'));
			}
			else
			{
				throw new ArgumentException("totalWidth must be even.", "totalWidth");
			}
		}

		/// <summary>
		/// Gets a single byte out of a BCD encoded string.
		/// </summary>
		/// <param name="s">The string containing the BCD data.</param>
		/// <param name="index">The position in the string to start.</param>
		/// <returns>The byte at the specified position.</returns>
		/// <remarks>No range checking is performed.</remarks>
		public static byte GetByte(string s, int index)
		{
			string str = s.Substring(index * 2, 2);
			return Calc.HexToInt(str)[0];
		}

		/// <summary>
		/// Gets multiple bytes out of a BCD encoded string.
		/// </summary>
		/// <param name="s">The string containing the BCD data.</param>
		/// <param name="index">The position in the string to start.</param>
		/// <param name="length">The number of bytes to read.</param>
		/// <returns>The bytes within the specified range.</returns>
		/// <remarks>No range checking is performed.</remarks>
		public static byte[] GetBytes(string s, int index, int length)
		{
			string str = s.Substring(index * 2, length * 2);
			return Calc.HexToInt(str);
		}

		/// <summary>
		/// Gets multiple bytes as string out of a BCD encoded string.
		/// </summary>
		/// <param name="s">The string containing the BCD data.</param>
		/// <param name="index">The position in the string to start.</param>
		/// <param name="length">The number of bytes to read.</param>
		/// <returns>The bytes within the specified range.</returns>
		/// <remarks>No range checking is performed.</remarks>
		public static string GetBytesString(string s, int index, int length)
		{
			return s.Substring(index * 2, length * 2);
		}

		/// <summary>
		/// Gets a single byte as string out of a BCD encoded string.
		/// </summary>
		/// <param name="s">The string containing the BCD data.</param>
		/// <param name="index">The byte at the specified position.</param>
		/// <returns>The byte at the specified position.</returns>
		/// <remarks>No range checking is performed.</remarks>
		public static string GetByteString(string s, int index)
		{
			return s.Substring(index * 2, 2);
		}
	}
}