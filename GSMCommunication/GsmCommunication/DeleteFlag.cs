/// <summary>
/// Lists the possible delete flags for AT+CMGD.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum DeleteFlag
	{
		/// <summary>Delete the message specified in index.</summary>
		DeleteSpecified,
		/// <summary>
		/// Delete all read messages from preferred message storage, leaving unread messages and stored mobile
		/// originated messages (whether sent or not) untouched.
		/// </summary>
		DeleteRead,
		/// <summary>
		/// Delete all read messages from preferred message storage and sent mobile originated messages,
		/// leaving unread messages and unsent mobile originated messages untouched.
		/// </summary>
		DeleteReadAndSent,
		/// <summary>
		/// Delete all read messages from preferred message storage, sent and unsent mobile originated messages
		/// leaving unread messages untouched.
		/// </summary>
		DeleteReadSentAndUnsent,
		/// <summary>
		/// Delete all messages from preferred message storage including unread messages.
		/// </summary>
		DeleteAll
	}
}