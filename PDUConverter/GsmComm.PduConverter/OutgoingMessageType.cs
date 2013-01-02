/// <summary>
/// Specifies the type of the outgoing message.
/// </summary>
namespace GsmComm.PduConverter
{
	public enum OutgoingMessageType
	{
		/// <summary>Specifies that the message is an SMS-SUBMIT.</summary>
		SmsSubmit,
		/// <summary>Specifies that the message is an SMS-COMMAND.</summary>
		SmsCommand,
		/// <summary>Specifies that the message is an SMS-DELIVER-REPORT.</summary>
		SmsDeliverReport
	}
}