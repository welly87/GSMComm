using System;

/// <summary>
/// Implements an Application Port Addressing Information Element (16 bit address).
/// </summary>
/// <remarks>This element is used to indiate from which port a message 
/// originated and to which port it should be directed to.</remarks>
namespace GsmComm.PduConverter.SmartMessaging
{
	public class PortAddressElement16 : InformationElement
	{
		/// <summary>
		/// The Information Element Identifier (IEI).
		/// </summary>
		public const byte Identifier = 5;

		private ushort destinationPort;

		private ushort originatorPort;

		/// <summary>
		/// Gets the destination port.
		/// </summary>
		public ushort DestinationPort
		{
			get
			{
				return this.destinationPort;
			}
		}

		/// <summary>
		/// Gets the originator port.
		/// </summary>
		public ushort OriginatorPort
		{
			get
			{
				return this.originatorPort;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmartMessaging.PortAddressElement16" /> class.
		/// </summary>
		/// <param name="destinationPort">The destination port, e.g. 0x1582.</param>
		/// <param name="originatorPort">The source port, e.g. 0x00.</param>
		public PortAddressElement16(ushort destinationPort, ushort originatorPort)
		{
			this.destinationPort = destinationPort;
			this.originatorPort = originatorPort;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmartMessaging.PortAddressElement16" /> class.
		/// </summary>
		/// <param name="element">The information element as a byte array.</param>
		public PortAddressElement16(byte[] element)
		{
			if (element != null)
			{
				if (element[0] == 5)
				{
					byte num = element[1];
					if (num >= 4)
					{
						byte[] numArray = new byte[2];
						numArray[0] = element[3];
						numArray[1] = element[2];
						this.destinationPort = BitConverter.ToUInt16(numArray, 0);
						byte[] numArray1 = new byte[2];
						numArray1[0] = element[5];
						numArray1[1] = element[4];
						this.originatorPort = BitConverter.ToUInt16(numArray1, 0);
						return;
					}
					else
					{
						throw new FormatException("Information element data must be 4 bytes long.");
					}
				}
				else
				{
					throw new ArgumentException("Element is not an Application Port Addressing Information Element (16 bit address).", "element");
				}
			}
			else
			{
				throw new ArgumentNullException("element");
			}
		}

		/// <summary>
		/// Returns the byte array equivalent of this instance.
		/// </summary>
		/// <returns>The byte array.</returns>
		public override byte[] ToByteArray()
		{
			byte[] bytes = BitConverter.GetBytes(this.destinationPort);
			byte[] numArray = BitConverter.GetBytes(this.originatorPort);
			byte[] numArray1 = new byte[6];
			numArray1[0] = 5;
			numArray1[1] = 4;
			numArray1[2] = bytes[1];
			numArray1[3] = bytes[0];
			numArray1[4] = numArray[1];
			numArray1[5] = numArray[0];
			return numArray1;
		}
	}
}