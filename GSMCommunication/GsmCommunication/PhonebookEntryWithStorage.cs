using System;
using System.Xml.Serialization;

/// <summary>
/// Represents a <see cref="T:GsmComm.GsmCommunication.PhonebookEntry" /> extended by the storage value.
/// </summary>
namespace GsmComm.GsmCommunication
{
	[Serializable]
	public class PhonebookEntryWithStorage : PhonebookEntry
	{
		private string storage;

		/// <summary>
		/// The storage the entry was read from.
		/// </summary>
		[XmlAttribute]
		public string Storage
		{
			get
			{
				return this.storage;
			}
			set
			{
				this.storage = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public PhonebookEntryWithStorage()
		{
		}

		/// <summary>
		/// Initializes a new instance of the class using the specified values.
		/// </summary>
		/// <param name="entry">The phonebook entry</param>
		/// <param name="storage">The storage the entry was read from.</param>
		public PhonebookEntryWithStorage(PhonebookEntry entry, string storage) : base(entry)
		{
			this.storage = storage;
		}
	}
}