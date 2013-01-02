/// <summary>
/// Contains the possible formats in which a network operator can be returned.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum OperatorFormat
	{
		/// <summary>Long format, alphanumeric</summary>
		LongFormatAlphanumeric,
		/// <summary>Short format, alphanumeric</summary>
		ShortFormatAlphanumeric,
		/// <summary>
		/// Numeric format, GSM Location Area Identification number (BCD encoded, 3 digits country code,
		/// 2 digits network code)
		/// </summary>
		Numeric
	}
}