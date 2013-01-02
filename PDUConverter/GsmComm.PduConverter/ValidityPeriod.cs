/// <summary>
/// Implements the base for a validity Period.
/// </summary>
/// <remarks><para>
/// <b>Note to inheritors:</b> Override the ToString method in derived classes to be able to
/// display the validity immediately as a string.
/// </para>
/// </remarks>
namespace GsmComm.PduConverter
{
	public abstract class ValidityPeriod
	{
		protected ValidityPeriod()
		{
		}
	}
}