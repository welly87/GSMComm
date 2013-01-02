/// <summary>
/// Contains status information of all message memories.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class MessageMemoryStatus
	{
		private MemoryStatus readStorage;

		private MemoryStatus writeStorage;

		private MemoryStatus receiveStorage;

		/// <summary>
		/// Gets or sets the status of the current read storage.
		/// </summary>
		public MemoryStatus ReadStorage
		{
			get
			{
				return this.readStorage;
			}
			set
			{
				this.readStorage = value;
			}
		}

		/// <summary>
		/// Gets or sets the status of the current receive storage.
		/// </summary>
		public MemoryStatus ReceiveStorage
		{
			get
			{
				return this.receiveStorage;
			}
			set
			{
				this.receiveStorage = value;
			}
		}

		/// <summary>
		/// Gets or sets the status of the current write storage.
		/// </summary>
		public MemoryStatus WriteStorage
		{
			get
			{
				return this.writeStorage;
			}
			set
			{
				this.writeStorage = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public MessageMemoryStatus()
		{
		}

		/// <summary>
		/// Initializes a new instance of the class with the specified parameters.
		/// </summary>
		/// <param name="readStorage">Status of the current read storage</param>
		/// <param name="writeStorage">Status of the current write storage</param>
		/// <param name="receiveStorage">Status of the current receive storage</param>
		public MessageMemoryStatus(MemoryStatus readStorage, MemoryStatus writeStorage, MemoryStatus receiveStorage)
		{
			this.readStorage = readStorage;
			this.writeStorage = writeStorage;
			this.receiveStorage = receiveStorage;
		}
	}
}