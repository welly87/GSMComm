using System;

/// <summary>
/// Reserved coding
/// </summary>
namespace GsmComm.PduConverter
{
	public class ReservedCodingGroup : DataCodingScheme
	{
		private byte dcs;

		/// <summary>
		/// Gets the alphabet being used.
		/// </summary>
		public override byte Alphabet
		{
			get
			{
				return 3;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="dcs">The DCS byte to decode.</param>
		public ReservedCodingGroup(byte dcs) : base(dcs)
		{
			this.dcs = dcs;
		}
	}
}