using System;

/// <summary>
/// Contains the memory status of a specific storage, including the storage type.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class MemoryStatusWithStorage : MemoryStatus
	{
		private string storage;

		/// <summary>
		/// Gets the storage that this memory status applies to.
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
		/// <param name="storage">The storage that this memory status applies to</param>
		/// <param name="used">The number of messages in the storage</param>
		/// <param name="total">The total capacity of the storage</param>
		public MemoryStatusWithStorage(string storage, int used, int total) : base(used, total)
		{
			this.storage = storage;
		}
	}
}