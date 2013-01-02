using System;

/// <summary>
/// Message Waiting Indication Group: Discard Message
/// </summary>
namespace GsmComm.PduConverter
{
	public class MessageWaitingDiscard : MessageWaitingIndication
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
		public MessageWaitingDiscard(byte dcs) : base(dcs)
		{
		}
	}
}