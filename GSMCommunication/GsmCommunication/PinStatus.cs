/// <summary>
/// Lists the possible PIN states of the phone.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public enum PinStatus
	{
		/// <summary>Phone does not wait for any password</summary>
		Ready,
		/// <summary>Phone is waiting for SIM PIN to be given</summary>
		SimPin,
		/// <summary>Phone is waiting for SIM PUK to be given</summary>
		SimPuk,
		/// <summary>Phone is waiting for phone to SIM card password to be given</summary>
		PhoneToSimPin,
		/// <summary>Phone is waiting for phone-to-very first SIM card password to be given</summary>
		PhoneToFirstSimPin,
		/// <summary>Phone is waiting for phone-to-very first SIM card unblocking password to be given</summary>
		PhoneToFirstSimPuk,
		/// <summary>
		/// Phone is waiting for SIM PIN2 to be given (this status should be expected to be returned
		/// by phones only when the last executed command resulted in PIN2 authentication failure (i.e. device
		/// error 17); if PIN2 is not entered right after the failure, the phone should be expected not to block
		/// its operation)
		/// </summary>
		SimPin2,
		/// <summary>
		/// Phone is waiting for SIM PUK2 to be given (this status should be expected to be returned
		/// by phones only when the last executed command resulted in PUK2 authentication failure (i.e. device
		/// error 18); if PUK2 is not entered right after the failure, the phone should be expected not to block
		/// its operation)
		/// </summary>
		SimPuk2,
		/// <summary>Phone is waiting for network personalization password to be given</summary>
		PhoneToNetworkPin,
		/// <summary>Phone is waiting for network personalization unblocking password to be given</summary>
		PhoneToNetworkPuk,
		/// <summary>Phone is waiting for network subset personalization password to be given</summary>
		PhoneToNetworkSubsetPin,
		/// <summary>Phone is waiting for network subset personalization unblocking password to be given</summary>
		PhoneToNetworkSubsetPuk,
		/// <summary>Phone is waiting for service provider personalization password to be given</summary>
		PhoneToServiceProviderPin,
		/// <summary>Phone is waiting for service provider personalization unblocking password to be given</summary>
		PhoneToServiceProviderPuk,
		/// <summary>Phone is waiting for corporate personalization password to be given</summary>
		PhoneToCorporatePin,
		/// <summary>Phone is waiting for corporate personalization unblocking password to be given</summary>
		PhoneToCorporatePuk
	}
}