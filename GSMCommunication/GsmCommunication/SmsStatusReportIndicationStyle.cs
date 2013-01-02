/// <summary>
/// Specifies the possible indication settings for new status report messages.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum SmsStatusReportIndicationStyle
	{
		/// <summary>
		/// No SMS-STATUS-REPORTs are routed to the TE.
		/// </summary>
		Disabled,
		/// <summary>
		/// SMS-STATUS-REPORTs are routed to the TE using unsolicited result code. Depending on the currently
		/// selected message format, this is done in either PDU or text mode.
		/// </summary>
		RouteMessage,
		/// <summary>
		/// If SMS-STATUS-REPORT is stored into ME/TA, indication of the memory location is routed to the TE.
		/// </summary>
		RouteMemoryLocation
	}
}