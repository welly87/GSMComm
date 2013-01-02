using System;

/// <summary>
/// A common interface for all information elements containing concatenation information.
/// </summary>
namespace GsmComm.PduConverter.SmartMessaging
{
	public interface IConcatenationInfo
	{
		/// <summary>
		/// Gets the current message number.
		/// </summary>
		byte CurrentNumber
		{
			get;
		}

		/// <summary>
		/// Gets the message reference number.
		/// </summary>
		ushort ReferenceNumber
		{
			get;
		}

		/// <summary>
		/// Gets the total number of parts of the message.
		/// </summary>
		byte TotalMessages
		{
			get;
		}

	}
}