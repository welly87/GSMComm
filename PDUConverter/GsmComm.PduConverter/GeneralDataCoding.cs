using System;

/// <summary>
/// General Data Coding indication
/// </summary>
namespace GsmComm.PduConverter
{
	public class GeneralDataCoding : DataCodingScheme
	{
		private bool compressed;

		private bool classSpecified;

		private byte alphabet;

		private byte messageClass;

		/// <summary>
		/// Gets the alphabet being used.
		/// </summary>
		public override byte Alphabet
		{
			get
			{
				return this.alphabet;
			}
		}

		/// <summary>
		/// Determines if the <see cref="P:GsmComm.PduConverter.GeneralDataCoding.MessageClass" /> property has a message class meaning. If not,
		/// the <see cref="P:GsmComm.PduConverter.GeneralDataCoding.MessageClass" /> property contains a reserved value and has no message class meaning.
		/// </summary>
		public bool ClassSpecified
		{
			get
			{
				return this.classSpecified;
			}
		}

		/// <summary>
		/// Gets whether the text is compressed.
		/// </summary>
		public bool Compressed
		{
			get
			{
				return this.compressed;
			}
		}

		/// <summary>
		/// Gets the message class.
		/// </summary>
		public byte MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="dcs">The DCS byte to decode.</param>
		public GeneralDataCoding(byte dcs) : base(dcs)
		{
			this.compressed = (dcs & 32) > 0;
			this.classSpecified = (dcs & 16) > 0;
			this.alphabet = (byte)(dcs >> 2 & 3);
			this.messageClass = (byte)(dcs & 3);
		}
	}
}