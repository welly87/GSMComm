using System;

/// <summary>
/// Contains network address data.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class AddressData
	{
		private string address;

		private int typeOfAddress;

		/// <summary>
		/// Gets the network address.
		/// </summary>
		public string Address
		{
			get
			{
				return this.address;
			}
		}

		/// <summary>
		/// Gets the type of the <see cref="P:GsmComm.GsmCommunication.AddressData.Address" />.
		/// </summary>
		public int TypeOfAddress
		{
			get
			{
				return this.typeOfAddress;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="address">The network address</param>
		/// <param name="typeOfAddress">The type of the given <b>address</b></param>
		public AddressData(string address, int typeOfAddress)
		{
			this.address = address;
			this.typeOfAddress = typeOfAddress;
		}
	}
}