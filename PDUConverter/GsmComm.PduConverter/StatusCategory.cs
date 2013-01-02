/// <summary>
/// Contains the possible categories of a message status.
/// </summary>
namespace GsmComm.PduConverter
{
	public enum StatusCategory
	{
		/// <summary>Short message transaction completed.</summary>
		Success,
		/// <summary>Temporary error, SC still trying to transfer SM.</summary>
		TemporaryErrorWithRetry,
		/// <summary>Permanent error, SC is not making any more transfer attempts.</summary>
		PermanentError,
		/// <summary>Temporary error, SC is not making any more transfer attempts.</summary>
		TemporaryErrorNoRetry,
		/// <summary>Status code is out of the defined range.</summary>
		Reserved
	}
}