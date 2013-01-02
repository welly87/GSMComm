using System;

/// <summary>
/// The base class for the message flags of incoming messages.
/// </summary>
namespace GsmComm.PduConverter
{
	public abstract class IncomingMessageFlags
	{
		/// <summary>
		/// Gets the message type.
		/// </summary>
		public abstract IncomingMessageType MessageType
		{
			get;
		}

		protected IncomingMessageFlags()
		{
		}

		/// <summary>
		/// In derived classes, converts the specified <see cref="T:System.Byte" /> value into a new instance of the <see cref="T:GsmComm.PduConverter.IncomingMessageFlags" /> class.
		/// </summary>
		/// <param name="b">A <see cref="T:System.Byte" /> value.</param>
		protected abstract void FromByte(byte b);

		public static implicit operator Byte(IncomingMessageFlags flags)
		{
			return flags.ToByte();
		}

		/// <summary>
		/// In derived classes, returns the byte equivalent of this instance.
		/// </summary>
		/// <returns>The byte value.</returns>
		public abstract byte ToByte();

		/// <summary>
		/// Returns the string equivalent of this instance.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			byte num = this.ToByte();
			return num.ToString();
		}
	}
}