using System;
using System.Xml.Serialization;

/// <summary>
/// Represents a phonebook entry.
/// </summary>
namespace GsmComm.GsmCommunication
{
	[Serializable]
	[XmlInclude(typeof(PhonebookEntryWithStorage))]
	public class PhonebookEntry
	{
		private int index;

		private string number;

		private int type;

		private string text;

		/// <summary>
		/// The index where the entry is saved in the phone.
		/// </summary>
		[XmlAttribute]
		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		/// <summary>
		/// The phone number.
		/// </summary>
		[XmlAttribute]
		public string Number
		{
			get
			{
				return this.number;
			}
			set
			{
				this.number = value;
			}
		}

		/// <summary>
		/// The text (name) associated with the <see cref="P:GsmComm.GsmCommunication.PhonebookEntry.Number" />.
		/// </summary>
		[XmlAttribute]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		/// <summary>
		/// The <see cref="P:GsmComm.GsmCommunication.PhonebookEntry.Number" />'s address type.
		/// </summary>
		[XmlAttribute]
		public int Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public PhonebookEntry()
		{
		}

		/// <summary>
		/// Initializes a new instance of the class using the specified values.
		/// </summary>
		/// <param name="index">The index where the entry is saved in the phone.</param>
		/// <param name="number">The phone number.</param>
		/// <param name="type">The <see cref="P:GsmComm.GsmCommunication.PhonebookEntry.Number" />'s address type.</param>
		/// <param name="text">The text (name) associated with the <see cref="P:GsmComm.GsmCommunication.PhonebookEntry.Number" />.</param>
		public PhonebookEntry(int index, string number, int type, string text)
		{
			this.index = index;
			this.number = number;
			this.type = type;
			this.text = text;
		}

		/// <summary>
		/// Initializes a new instance of the class to copy an existing <see cref="T:GsmComm.GsmCommunication.PhonebookEntry" />.
		/// </summary>
		/// <param name="entry">The entry to copy.</param>
		public PhonebookEntry(PhonebookEntry entry)
		{
			this.index = entry.Index;
			this.number = entry.Number;
			this.type = entry.type;
			this.text = entry.Text;
		}
	}
}