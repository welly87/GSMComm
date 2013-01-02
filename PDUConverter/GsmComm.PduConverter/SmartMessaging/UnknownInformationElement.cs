using System;

/// <summary>
/// Implements an unknown information element.
/// </summary>
namespace GsmComm.PduConverter.SmartMessaging
{
	public class UnknownInformationElement : InformationElement
	{
		private byte identifier;

		private byte[] data;

		/// <summary>
		/// Gets the information element data.
		/// </summary>
		public byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		/// <summary>
		/// Gets the information element identifier.
		/// </summary>
		public byte Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmartMessaging.UnknownInformationElement" /> class.
		/// </summary>
		/// <param name="identifier">The information element identifier.</param>
		/// <param name="data">The information element data.</param>
		/// <exception cref="T:System.ArgumentNullException">data is null.</exception>
		/// <exception cref="T:System.ArgumentException">data is larger than 255 bytes.</exception>
		public UnknownInformationElement(byte identifier, byte[] data)
		{
			if (data != null)
			{
				if ((int)data.Length <= 255)
				{
					this.identifier = identifier;
					this.data = data;
					return;
				}
				else
				{
					throw new ArgumentException("Data must be between 0 and 255 bytes long.", "data");
				}
			}
			else
			{
				throw new ArgumentNullException("data");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.SmartMessaging.UnknownInformationElement" /> class.
		/// </summary>
		/// <param name="element">The information element as a byte array.</param>
		/// <exception cref="T:System.ArgumentNullException">element is null.</exception>
		/// <exception cref="T:System.ArgumentException">Information element is shorter than 2 bytes.</exception>
		/// <exception cref="T:System.FormatException">Available number of data bytes is less than specified in data length.</exception>
		public UnknownInformationElement(byte[] element)
		{
			if (element != null)
			{
				if ((int)element.Length >= 2)
				{
					this.identifier = element[0];
					byte num = element[1];
					if ((int)element.Length >= num + 2)
					{
						this.data = new byte[num];
						Array.Copy(element, 2, this.data, 0, num);
						return;
					}
					else
					{
						throw new FormatException("Available number of data bytes is less then specified in data length.");
					}
				}
				else
				{
					throw new ArgumentException("Information element must at least be 2 bytes long.", "element");
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
			byte[] length = new byte[2 + (int)this.data.Length];
			length[0] = this.identifier;
			length[1] = (byte)((int)this.data.Length);
			this.data.CopyTo(length, 2);
			return length;
		}
	}
}