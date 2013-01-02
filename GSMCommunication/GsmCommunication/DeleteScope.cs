/// <summary>
/// Lists the possible scopes for deleting short messages.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum DeleteScope
	{
		/// <summary>Delete all read messages.</summary>
		Read = 1,
		/// <summary>Delete all read and sent messages.</summary>
		ReadAndSent = 2,
		/// <summary>Delete all read, sent and unsent messages</summary>
		ReadSentAndUnsent = 3,
		/// <summary>Delete all messages including unread messages.</summary>
		All = 4
	}
}