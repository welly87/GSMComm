using System;

/// <summary>
/// Lists some common character sets.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class Charset
	{
		/// <summary>The UCS2 (Unicode) character set</summary>
		public const string Ucs2 = "UCS2";

		/// <summary>The GSM character set</summary>
		public const string Gsm = "GSM";

		/// <summary>The PCCP437 character set</summary>
		public const string Pccp437 = "PCCP437";

		/// <summary>The PCDN character set</summary>
		public const string Pcdn = "PCDN";

		/// <summary>The IRA character set</summary>
		public const string Ira = "IRA";

		/// <summary>The ISO 8859-1 character set</summary>
		public const string Iso8859_1 = "8859-1";

		/// <summary>The characters encoded as hex</summary>
		public const string Hex = "HEX";

		public Charset()
		{
		}
	}
}