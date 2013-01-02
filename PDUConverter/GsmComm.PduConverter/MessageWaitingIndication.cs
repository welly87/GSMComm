using System;

/// <summary>
/// Message waiting indication. This class is abstract.
/// </summary>
namespace GsmComm.PduConverter
{
	public abstract class MessageWaitingIndication : DataCodingScheme
	{
		private bool indicationActive;

		private bool bit2;

		private byte indicationType;

		/// <summary>
		/// Gets if the indication should be set active.
		/// </summary>
		/// <remarks>If true, the indication should be set active, if false, the indication should be set inactive.</remarks>
		public bool IndicationActive
		{
			get
			{
				return this.indicationActive;
			}
		}

		/// <summary>
		/// Gets the indication type, how the indication should be shown.
		/// </summary>
		public byte IndicationType
		{
			get
			{
				return this.indicationType;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="dcs">The DCS byte to decode.</param>
		public MessageWaitingIndication(byte dcs) : base(dcs)
		{
			this.indicationType = (byte)(dcs & 3);
			this.bit2 = (dcs & 4) > 0;
			this.indicationActive = (dcs & 8) > 0;
		}
	}
}