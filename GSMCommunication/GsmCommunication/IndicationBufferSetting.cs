/// <summary>
/// Specifies what should happen to the TA's indication buffer.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum IndicationBufferSetting
	{
		/// <summary>
		/// The TA buffer of unsolicited result codes is flushed to the TE when an
		/// <see cref="T:GsmComm.GsmCommunication.MessageIndicationMode" /> other than <see cref="F:GsmComm.GsmCommunication.MessageIndicationMode.DoNotForward" /> is entered.
		/// </summary>
		Flush,
		/// <summary>
		/// TA buffer of unsolicited result codes defined within this command is cleared when an
		/// <see cref="T:GsmComm.GsmCommunication.MessageIndicationMode" /> other than <see cref="F:GsmComm.GsmCommunication.MessageIndicationMode.DoNotForward" /> is entered.
		/// </summary>
		Clear
	}
}