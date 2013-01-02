using System;

/// <summary>
/// Contains information about a GSM network operator.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class OperatorInfo2
	{
		private OperatorStatus stat;

		private string longAlpha;

		private string shortAlpha;

		private string numeric;

		private string act;

		/// <summary>
		/// Gets the access technology the operator uses.
		/// </summary>
		/// <remarks>This is optional, as it is only useful for terminals capable to register to more than
		/// one access technology.</remarks>
		public string AccessTechnology
		{
			get
			{
				return this.act;
			}
		}

		/// <summary>
		/// Gets the operator name in long alphanumeric format.
		/// </summary>
		/// <remarks>If the phone does not support this format, the string will be empty.</remarks>
		public string LongAlphanumeric
		{
			get
			{
				return this.longAlpha;
			}
		}

		/// <summary>
		/// Gets the operator in numeric format.
		/// </summary>
		/// <remarks>If the phone does not support this format, the string will be empty.</remarks>
		public string Numeric
		{
			get
			{
				return this.numeric;
			}
		}

		/// <summary>
		/// Gets the operator name in short alphanumic format.
		/// </summary>
		/// <remarks>If the phone does not support this format, the string will be empty.</remarks>
		public string ShortAlphanumeric
		{
			get
			{
				return this.shortAlpha;
			}
		}

		/// <summary>
		/// Gets the availability of the operator.
		/// </summary>
		public OperatorStatus Status
		{
			get
			{
				return this.stat;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="status">The operator availability</param>
		/// <param name="longAlphanumeric">The operator name in long alphanumeric format</param>
		/// <param name="shortAlphanumeric">The operator name in short alphanumeric format</param>
		/// <param name="numeric">The operator in numeric format</param>
		/// <remarks>If the phone does not support one of the formats <b>longAlphanumeric</b>,
		/// <b>shortAlphanumeric</b>, <b>numeric</b>, the curresponding string is left empty.</remarks>
		public OperatorInfo2(OperatorStatus status, string longAlphanumeric, string shortAlphanumeric, string numeric)
		{
			this.stat = status;
			this.longAlpha = longAlphanumeric;
			this.shortAlpha = shortAlphanumeric;
			this.numeric = numeric;
			this.act = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="status">The operator availability</param>
		/// <param name="longAlphanumeric">The operator name in long alphanumeric format</param>
		/// <param name="shortAlphanumeric">The operator name in short alphanumeric format</param>
		/// <param name="numeric">The operator in numeric format</param>
		/// <param name="accessTechnology">The access technology the operator uses.</param>
		/// <remarks>
		/// <para>If the phone does not support one of the formats <b>longAlphanumeric</b>,
		/// <b>shortAlphanumeric</b>, <b>numeric</b>, the curresponding string is left empty.</para>
		/// <para>The <b>accessTechnology</b> is optional, as it is only useful for terminals capable
		/// to register to more than one access technology.</para>
		/// </remarks>
		public OperatorInfo2(OperatorStatus status, string longAlphanumeric, string shortAlphanumeric, string numeric, string accessTechnology)
		{
			this.stat = status;
			this.longAlpha = longAlphanumeric;
			this.shortAlpha = shortAlphanumeric;
			this.numeric = numeric;
			this.act = accessTechnology;
		}
	}
}