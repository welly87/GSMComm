/// <summary>
/// Contains the possible modes for the AT+CMMS command to set the high-speed SMS sending behaviour.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum MoreMessagesMode
	{
		/// <summary>The function is disabled, the SMS link is not kept open.</summary>
		Disabled,
		/// <summary>
		/// Keep enabled until the time between the response of the latest message send command (+CMGS, +CMSS, etc.)
		/// and the next send command exceeds 1-5 seconds (the exact value is up to ME implementation), then ME shall
		/// close the link and TA switches the mode automatically back to disabled (0).
		/// </summary>
		Temporary,
		/// <summary>
		/// Enables (if the time between the response of the latest message send command and the next send command
		/// exceeds 1-5 seconds (the exact value is up to ME implementation), ME shall close the link but TA shall
		/// not switch automatically back to disabled (0)).
		/// </summary>
		Permanent
	}
}