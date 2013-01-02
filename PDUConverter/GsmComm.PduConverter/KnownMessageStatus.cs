using System;

/// <summary>
/// This enumarator represents the known status codes of a TP-ST octet.
/// Reserved and SC specific values are not part of this list.
/// </summary>
namespace GsmComm.PduConverter
{
	public enum KnownMessageStatus : byte
	{
		/// <summary>Short message received by the SME.</summary>
		OK_Received,
		/// <summary>Short message forwarded by the SC to the SME but the SC is	unable to confirm delivery.</summary>
		OK_NotConfirmed,
		/// <summary>Short message replaced by the SC.</summary>
		OK_Replaced,
		/// <summary>Congestion.</summary>
		Temp_Congestion,
		/// <summary>SME busy.</summary>
		Temp_SmeBusy,
		/// <summary>No response from SME.</summary>
		Temp_NoResponseFromSme,
		/// <summary>Service Rejected.</summary>
		Temp_ServiceRejected,
		/// <summary>Quality of service not available.</summary>
		Temp_QosNotAvailable,
		/// <summary>Error in SME.</summary>
		Temp_ErrorInSme,
		/// <summary>Remote procedure error.</summary>
		Perm_RemoteProcedureError,
		/// <summary>Incompatible destination.</summary>
		Perm_IncompatibleDestination,
		/// <summary>Connection rejected by SME.</summary>
		Perm_ConnectionRejectedBySme,
		/// <summary>Not obtainable.</summary>
		Perm_NotObtainable,
		/// <summary>Quality of service not available.</summary>
		Perm_QosNotAvailable,
		/// <summary>No interworking available.</summary>
		Perm_NoInterworkingAvailable,
		/// <summary>SM Validity Period expired.</summary>
		Perm_SMValidityPeriodExpired,
		/// <summary>SM Deleted by originating SME.</summary>
		Perm_SMDeletedByOriginatingSme,
		/// <summary>SM Deleted by SC Administration.</summary>
		Perm_SMDeletedBySCAdministration,
		/// <summary>SM does not exist (The SM may have previously existed in the SC but the
		/// SC no longer has knowledge of it or the SM may never have previously existed in the SC).</summary>
		Perm_SMDoesNotExist,
		/// <summary>Congestion.</summary>
		Ntemp_Congestion,
		/// <summary>SME busy.</summary>
		Ntemp_SmeBusy,
		/// <summary>No response from SME.</summary>
		Ntemp_NoResponseFromSme,
		/// <summary>Service rejected.</summary>
		Ntemp_ServiceRejected,
		/// <summary>Quality of service not available.</summary>
		Ntemp_QosNotAvailable,
		/// <summary>Error in SME.</summary>
		Ntemp_ErrorInSme
	}
}