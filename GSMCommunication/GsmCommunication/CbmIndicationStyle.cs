/// <summary>
/// Specifies the possible indication settings for new cell broadcast messages (CBMs).
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum CbmIndicationStyle
	{
		/// <summary>
		/// No CBM indications are routed to the TE.
		/// </summary>
		Disabled,
		/// <summary>
		/// If CBM is stored into ME/TA, indication of the memory location is routed to the TE.
		/// </summary>
		RouteMemoryLocation,
		/// <summary>
		/// New CBMs are routed directly to the TE.
		/// </summary>
		/// <remarks> If ME supports data coding groups which define special routing also for messages other than
		/// class 3 (e.g. SIM specific messages), ME may choose not to route messages of such data coding schemes
		/// into TE (indication of a stored CBM may be given as with <see cref="F:GsmComm.GsmCommunication.CbmIndicationStyle.RouteMemoryLocation" />.</remarks>
		RouteMessage,
		/// <summary>
		/// Class 3 CBMs are routed directly to TE using the same indications as with <see cref="F:GsmComm.GsmCommunication.CbmIndicationStyle.RouteMessage" />.
		/// If CBM storage is supported, messages of other classes result in indication as with
		/// <see cref="F:GsmComm.GsmCommunication.CbmIndicationStyle.RouteMemoryLocation" />.
		/// </summary>
		RouteSpecial
	}
}