using System;

/// <summary>
/// Contains the memory status of a specific storage.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class MemoryStatus
	{
		private int used;

		private int total;

		/// <summary>
		/// Gets the total capacity of the storage.
		/// </summary>
		public int Total
		{
			get
			{
				return this.total;
			}
		}

		/// <summary>
		/// Gets the number of messages in the storage.
		/// </summary>
		public int Used
		{
			get
			{
				return this.used;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="used">The number of messages in the storage</param>
		/// <param name="total">The total capacity of the storage</param>
		public MemoryStatus(int used, int total)
		{
			this.used = used;
			this.total = total;
		}
	}
}