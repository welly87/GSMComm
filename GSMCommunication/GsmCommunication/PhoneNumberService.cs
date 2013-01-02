/// <summary>
/// Contains services related to a subscriber phone number.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum PhoneNumberService
	{
		/// <summary>Asynchronous modem</summary>
		AsynchronousModem,
		/// <summary>Synchronous modem</summary>
		SynchronousModem,
		/// <summary>PAD Access (asynchronous)</summary>
		PadAccess,
		/// <summary>Packet Access (synchronous)</summary>
		PacketAccess,
		/// <summary>Voice</summary>
		Voice,
		/// <summary>Fax</summary>
		Fax
	}
}