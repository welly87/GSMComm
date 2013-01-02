/// <summary>
/// Represents a common interface for messages to return their relevant
/// timestamp.
/// </summary>
namespace GsmComm.PduConverter
{
	public interface ITimestamp
	{
		/// <summary>
		/// Returns the relevant timestamp.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.PduConverter.SmsTimestamp" /> structure representing the relevant message timestamp.</returns>
		SmsTimestamp GetTimestamp();
	}
}