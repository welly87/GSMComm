using System;

/// <summary>
/// Contains the memory location of a saved message.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class MemoryLocation : IMessageIndicationObject
	{
		private string storage;

		private int index;

		/// <summary>
		/// Gets the message index within the specified <see cref="P:GsmComm.GsmCommunication.MemoryLocation.Storage" />.
		/// </summary>
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		/// <summary>
		/// Gets the storage where the message is saved.
		/// </summary>
		public string Storage
		{
			get
			{
				return this.storage;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="storage">The storage where the message is saved.</param>
		/// <param name="index">The message index within the specified storage.</param>
		public MemoryLocation(string storage, int index)
		{
			this.storage = storage;
			this.index = index;
		}
	}
}