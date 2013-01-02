/// <summary>
/// Specifies how the validity period is formatted.
/// </summary>
namespace GsmComm.PduConverter
{
	public enum ValidityPeriodFormat
	{
		/// <summary>No validity is specified, the TP-VP block will not be specified.</summary>
		Unspecified,
		/// <summary>A relative validity is specified in the TP-VP block.</summary>
		Relative,
		/// <summary>An absolute validity is specified in the TP-VP block.</summary>
		Absolute,
		/// <summary>An enhanced validity is specified in the TP-VP block.</summary>
		Enhanced
	}
}