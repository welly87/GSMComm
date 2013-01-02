using System;

/// <summary>
/// Message Waiting Indication Group: Store Message (UCS2)
/// </summary>
namespace GsmComm.PduConverter
{
	public class MessageWaitingStoreUcs2 : MessageWaitingIndication
	{
		/// <summary>
		/// Gets the alphabet being used.
		/// </summary>
		public override byte Alphabet
		{
			get
			{
				return 2;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="dcs">The DCS byte to decode.</param>
		public MessageWaitingStoreUcs2(byte dcs) : base(dcs)
		{
		}
	}
}