using System;

/// <summary>
/// Message Waiting Indication Group: Store Message
/// </summary>
namespace GsmComm.PduConverter
{
	public class MessageWaitingStore : MessageWaitingIndication
	{
		/// <summary>
		/// Gets the alphabet being used.
		/// </summary>
		public override byte Alphabet
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="dcs">The DCS byte to decode.</param>
		public MessageWaitingStore(byte dcs) : base(dcs)
		{
		}
	}
}