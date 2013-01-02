using System;

/// <summary>
/// Contains information that identify a mobile phone.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public struct IdentificationInfo
	{
		private string manufacturer;

		private string model;

		private string revision;

		private string serialNumber;

		/// <summary>
		/// Gets or sets the manufacturer.
		/// </summary>
		public string Manufacturer
		{
			get
			{
				return this.manufacturer;
			}
			set
			{
				this.manufacturer = value;
			}
		}

		/// <summary>
		/// Gets or sets the model.
		/// </summary>
		public string Model
		{
			get
			{
				return this.model;
			}
			set
			{
				this.model = value;
			}
		}

		/// <summary>
		/// Gets or sets the revision.
		/// </summary>
		public string Revision
		{
			get
			{
				return this.revision;
			}
			set
			{
				this.revision = value;
			}
		}

		/// <summary>
		/// Gets or sets the serial number.
		/// </summary>
		public string SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
			set
			{
				this.serialNumber = value;
			}
		}

	}
}