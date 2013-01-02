/// <summary>
/// Specifies the possible modes for for new message indications.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum MessageIndicationMode
	{
		/// <summary>
		/// Buffer unsolicited result codes in the TA. If TA result code buffer is full, indications can be
		/// buffered in some other place or the oldest indications may be discarded and replaced with the new
		/// received indications.
		/// </summary>
		DoNotForward,
		/// <summary>
		/// Discard indication and reject new received message unsolicited result codes when TA-TE link is
		/// reserved (e.g. in on-line data mode). Otherwise forward them directly to the TE.
		/// </summary>
		SkipWhenReserved,
		/// <summary>
		/// Buffer unsolicited result codes in the TA when TA-TE link is reserved (e.g. in on-line data mode) and
		/// flush them to the TE after reservation. Otherwise forward them directly to the TE.
		/// </summary>
		BufferAndFlush,
		/// <summary>
		/// Forward unsolicited result codes directly to the TE. TA-TE link specific inband technique used to
		/// embed result codes and data when TA is in on-line data mode.
		/// </summary>
		ForwardAlways
	}
}