using System;

/// <summary>
/// Contains information about a GSM network operator.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class OperatorInfo
	{
		private OperatorFormat format;

		private string theOperator;

		private string accessTechnology;

		/// <summary>
		/// Gets the access technology registered to.
		/// </summary>
		/// <remarks>This is optional, as it is only useful for terminals capable to register to more than
		/// one access technology.</remarks>
		public string AccessTechnology
		{
			get
			{
				return this.accessTechnology;
			}
		}

		/// <summary>
		/// Gets the format in which <see cref="P:GsmComm.GsmCommunication.OperatorInfo.TheOperator" /> is specified in.
		/// </summary>
		public OperatorFormat Format
		{
			get
			{
				return this.format;
			}
		}

		/// <summary>
		/// Gets the operator in the format specified by <see cref="P:GsmComm.GsmCommunication.OperatorInfo.Format" />.
		/// </summary>
		public string TheOperator
		{
			get
			{
				return this.theOperator;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="format">The format in which <b>theOperator</b> is specified in. See <see cref="T:GsmComm.GsmCommunication.OperatorFormat" />
		/// for a list of possible values.
		/// </param>
		/// <param name="theOperator">The operator in the format specified by <b>format</b></param>
		public OperatorInfo(OperatorFormat format, string theOperator)
		{
			this.format = format;
			this.theOperator = theOperator;
			this.accessTechnology = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="format">The format in which <b>theOperator</b> is specified in. See <see cref="T:GsmComm.GsmCommunication.OperatorFormat" />
		/// for a list of possible values.
		/// </param>
		/// <param name="theOperator">The operator in the format specified by <b>format</b></param>
		/// <param name="accessTechnology">The access technology registered to.</param>
		public OperatorInfo(OperatorFormat format, string theOperator, string accessTechnology)
		{
			this.format = format;
			this.theOperator = theOperator;
			this.accessTechnology = accessTechnology;
		}
	}
}