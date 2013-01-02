/// <summary>
/// Conatins the operator availability values.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum OperatorStatus
	{
		/// <summary>The operator status is unknown.</summary>
		Unknown,
		/// <summary>The operator is available for selection.</summary>
		Available,
		/// <summary>Denotes that this is the currently selected operator.</summary>
		Current,
		/// <summary>The phone must not connect to this operator.</summary>
		Forbidden
	}
}