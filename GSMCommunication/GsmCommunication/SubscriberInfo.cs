using System;

/// <summary>
/// Contains network subscriber info retrieved from the phone.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class SubscriberInfo
	{
		private string alpha;

		private string number;

		private int type;

		private int speed;

		private int service;

		private int itc;

		/// <summary>
		/// Gets an optional alphanumeric string associated with <see cref="P:GsmComm.GsmCommunication.SubscriberInfo.Number" />;
		/// used character set is the one selected with <see cref="M:GsmComm.GsmCommunication.GsmPhone.SelectCharacterSet(System.String)" />.
		/// </summary>
		/// <remarks>If the string is not defined, it is empty.</remarks>
		public string Alpha
		{
			get
			{
				return this.alpha;
			}
		}

		/// <summary>
		/// Gets a value for the information transfer capability.
		/// </summary>
		/// <remarks>Valid values are zero or greater, -1 means this info is not available.</remarks>
		public int Itc
		{
			get
			{
				return this.itc;
			}
		}

		/// <summary>
		/// Gets the phone number of format specified by <see cref="P:GsmComm.GsmCommunication.SubscriberInfo.Type" />.
		/// </summary>
		public string Number
		{
			get
			{
				return this.number;
			}
		}

		/// <summary>
		/// Gets the service related to the phone number.
		/// </summary>
		/// <remarks><para>Valid values are zero or greater, -1 means this info is not available.</para>
		/// <para>Some defined values can be found in the <see cref="T:GsmComm.GsmCommunication.PhoneNumberService" /> enumeration.</para>
		/// </remarks>
		public int Service
		{
			get
			{
				return this.service;
			}
		}

		/// <summary>
		/// Gets a value for the speed for data calls.
		/// </summary>
		/// <remarks>Valid values are zero or greater, -1 means this info is not available.</remarks>
		public int Speed
		{
			get
			{
				return this.speed;
			}
		}

		/// <summary>
		/// Gets the type of Address in integer format.
		/// </summary>
		public int Type
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="number">Phone number of format specified by <b>type</b>.</param>
		/// <param name="type">Type of address in integer format.</param>
		public SubscriberInfo(string number, int type)
		{
			this.alpha = string.Empty;
			this.number = number;
			this.type = type;
			this.speed = -1;
			this.service = -1;
			this.itc = -1;
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="alpha">An optional alphanumeric string associated with <b>number</b></param>
		/// <param name="number">Phone number of format specified by <b>type</b></param>
		/// <param name="type">Type of address in integer format</param>
		/// <param name="speed">A value for the speed of data calls</param>
		/// <param name="service">The service related to the phone number</param>
		/// <param name="itc">A value for the information transfer capability</param>
		/// <remarks>
		/// Valid values for <b>speed</b>, <b>service</b> and <b>itc</b> are zero or greater,
		/// set values to -1 where the information is not available.
		/// </remarks>
		public SubscriberInfo(string alpha, string number, int type, int speed, int service, int itc)
		{
			this.alpha = alpha;
			this.number = number;
			this.type = type;
			this.speed = speed;
			this.service = service;
			this.itc = itc;
		}
	}
}