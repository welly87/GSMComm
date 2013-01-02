/// <summary>
/// Contains the possible values for the operator selection mode.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum OperatorSelectionMode
	{
		/// <summary>
		/// The phone selects the operator automatically.
		/// </summary>
		Automatic = 0,
		/// <summary>
		/// A specific operator is selected. The phone does not attempt to select the operator automatically.
		/// </summary>
		Manual = 1,
		/// <summary>
		/// The phone is not registered to the network.
		/// </summary>
		Deregistered = 2,
		/// <summary>
		/// If manual selection fails, automatic mode is entered.
		/// </summary>
		ManualAutomatic = 4
	}
}