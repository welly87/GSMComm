/// <summary>
/// Specifies the possible indication styles for new SMS-DELIVER messages.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum SmsDeliverIndicationStyle
	{
		/// <summary>
		/// No SMS-DELIVER indications are routed to the TE.
		/// </summary>
		Disabled,
		/// <summary>
		/// If SMS-DELIVER is stored into ME/TA, indication of the memory location is routed to the TE.
		/// </summary>
		RouteMemoryLocation,
		/// <summary>
		/// SMS-DELIVERs (except class 2 messages and messages in the message waiting indication
		/// group (store message)) are routed directly to the TE. Depending on the currently selected message
		/// format, this is done in either PDU or text mode.
		/// Class 2 messages and messages in the message waiting indication group (store message) result in
		/// the same indication as with <see cref="F:GsmComm.GsmCommunication.SmsDeliverIndicationStyle.RouteMemoryLocation" />.
		/// </summary>
		RouteMessage,
		/// <summary>
		/// Class 3 SMS-DELIVERs are routed directly to TE with the same format as with <see cref="F:GsmComm.GsmCommunication.SmsDeliverIndicationStyle.RouteMessage" />.
		/// Messages of other data coding schemes result in indication as with <see cref="F:GsmComm.GsmCommunication.SmsDeliverIndicationStyle.RouteMemoryLocation" />.
		/// </summary>
		RouteSpecial
	}
}