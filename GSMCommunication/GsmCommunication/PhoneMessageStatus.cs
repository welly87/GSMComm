/// <summary>
/// The message status to request from the phone or the actual type.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum PhoneMessageStatus
	{
		/// <summary>The message was received, but not yet read.</summary>
		ReceivedUnread,
		/// <summary>The message was received and has been read.</summary>
		ReceivedRead,
		/// <summary>The message was stored, but had not been sent yet.</summary>
		StoredUnsent,
		/// <summary>The message was stored and sent.</summary>
		StoredSent,
		/// <summary>Specifies all status.</summary>
		All
	}
}