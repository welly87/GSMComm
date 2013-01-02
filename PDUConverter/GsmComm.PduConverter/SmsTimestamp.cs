using System;

/// <summary>
/// For TP-SCTS and all other timestamps that look the same.
/// </summary>
namespace GsmComm.PduConverter
{
	public struct SmsTimestamp : IComparable
	{
		private byte year;

		private byte month;

		private byte day;

		private byte hour;

		private byte minute;

		private byte second;

		private int timeZoneOffset;

		/// <summary>
		/// The value for an invalid or not present timestamp.
		/// </summary>
		public static SmsTimestamp None;

		/// <summary>
		/// Gets the day.
		/// </summary>
		public byte Day
		{
			get
			{
				return this.day;
			}
		}

		/// <summary>
		/// Gets the hour.
		/// </summary>
		public byte Hour
		{
			get
			{
				return this.hour;
			}
		}

		/// <summary>
		/// Gets the minute.
		/// </summary>
		public byte Minute
		{
			get
			{
				return this.minute;
			}
		}

		/// <summary>
		/// Gets the month.
		/// </summary>
		public byte Month
		{
			get
			{
				return this.month;
			}
		}

		/// <summary>
		/// Gets the second.
		/// </summary>
		public byte Second
		{
			get
			{
				return this.second;
			}
		}

		/// <summary>
		/// Gets the time zone offset as an integer.
		/// </summary>
		/// <remarks>One unit of this offset equals 15 minutes. If you don't need this actual value,
		/// consider using <see cref="P:GsmComm.PduConverter.SmsTimestamp.TimeZoneOffsetSpan" /> instead.</remarks>
		public int TimeZoneOffset
		{
			get
			{
				return this.timeZoneOffset;
			}
		}

		/// <summary>
		/// Gets the time zone offset as a string useful to append to the sortable date string.
		/// </summary>
		private string TimeZoneOffsetSortableString
		{
			get
			{
				string str;
				TimeSpan timeZoneOffsetSpan = this.TimeZoneOffsetSpan;
				if (timeZoneOffsetSpan.Ticks < (long)0)
				{
					str = "-";
				}
				else
				{
					str = "+";
				}
				string str1 = str;
				int num = Math.Abs(timeZoneOffsetSpan.Hours);
				string str2 = num.ToString().PadLeft(2, '0');
				int num1 = Math.Abs(timeZoneOffsetSpan.Minutes);
				string str3 = num1.ToString().PadLeft(2, '0');
				return string.Concat(str1, str2, ":", str3);
			}
		}

		/// <summary>
		/// Gets the time zone offset as a time span.
		/// </summary>
		public TimeSpan TimeZoneOffsetSpan
		{
			get
			{
				return TimeSpan.FromMinutes((double)(this.timeZoneOffset * 15));
			}
		}

		/// <summary>
		/// Gets the time zone offset as a string.
		/// </summary>
		public string TimeZoneOffsetString
		{
			get
			{
				string str;
				TimeSpan timeZoneOffsetSpan = this.TimeZoneOffsetSpan;
				if (timeZoneOffsetSpan.Ticks < (long)0)
				{
					str = "-";
				}
				else
				{
					str = "+";
				}
				string str1 = str;
				int num = Math.Abs(timeZoneOffsetSpan.Hours);
				string str2 = num.ToString().PadLeft(2, '0');
				int num1 = Math.Abs(timeZoneOffsetSpan.Minutes);
				string str3 = num1.ToString().PadLeft(2, '0');
				return string.Concat(str1, str2, str3);
			}
		}

		/// <summary>
		/// Gets the year.
		/// </summary>
		public byte Year
		{
			get
			{
				return this.year;
			}
		}

		static SmsTimestamp()
		{
			SmsTimestamp.None = new SmsTimestamp(new DateTime(1900, 1, 1), 0);
		}

		/// <summary>
		/// Decodes the timestamp out of a PDU stream.
		/// </summary>
		/// <param name="pdu">The string to get the timestamp from.</param>
		/// <param name="index">The current position in the string</param>
		public SmsTimestamp(string pdu, ref int index)
		{
			string bytesString = BcdWorker.GetBytesString(pdu, index, 7);
			index = index + 7;
			string str = BcdWorker.DecodeSemiOctets(bytesString);
			this.year = byte.Parse(str.Substring(0, 2));
			this.month = byte.Parse(str.Substring(2, 2));
			this.day = byte.Parse(str.Substring(4, 2));
			this.hour = byte.Parse(str.Substring(6, 2));
			this.minute = byte.Parse(str.Substring(8, 2));
			this.second = byte.Parse(str.Substring(10, 2));
			string str1 = str.Substring(12, 2);
			byte num = Calc.HexToInt(str1)[0];
			if ((num & 128) <= 0)
			{
				this.timeZoneOffset = int.Parse(str1);
				return;
			}
			else
			{
				num = (byte)(num & 127);
				this.timeZoneOffset = -num;
				return;
			}
		}

		/// <summary>
		/// Creates the timestamp using a DateTime object and an offset.
		/// </summary>
		/// <param name="timestamp">The timestamp to initialize this object with.</param>
		/// <param name="timeZoneOffset">The time zone offset for the specified timestamp.</param>
		public SmsTimestamp(DateTime timestamp, int timeZoneOffset)
		{
			int year = timestamp.Year;
			this.year = (byte)int.Parse(year.ToString().Substring(2, 2));
			this.month = (byte)timestamp.Month;
			this.day = (byte)timestamp.Day;
			this.hour = (byte)timestamp.Hour;
			this.minute = (byte)timestamp.Minute;
			this.second = (byte)timestamp.Second;
			this.timeZoneOffset = timeZoneOffset;
		}

		/// <summary>
		/// Compares this instance to a specified object and returns an indication of their relative values.
		/// </summary>
		/// <param name="value">An object to compare, or a null reference</param>
		/// <returns>
		/// <list type="bullet">
		/// <item>less than zero = the instance is less than value.</item>
		/// <item>zero = the instance is equal to value.</item>
		/// <item>greater than zero = The instance is grater than value -or- value is a null reference.</item>
		/// </list>
		/// </returns>
		/// <exception cref="T:System.ArgumentException">value is not an <see cref="T:GsmComm.PduConverter.SmsTimestamp" />.</exception>
		public int CompareTo(object value)
		{
			if (!(value is SmsTimestamp))
			{
				if (value != null)
				{
					throw new ArgumentException("value is not an SmsTimestamp.");
				}
				else
				{
					return 1;
				}
			}
			else
			{
				SmsTimestamp smsTimestamp = (SmsTimestamp)value;
				int num = this.year.CompareTo(smsTimestamp.Year);
				if (num == 0)
				{
					num = this.month.CompareTo(smsTimestamp.Month);
					if (num == 0)
					{
						num = this.day.CompareTo(smsTimestamp.Day);
						if (num == 0)
						{
							num = this.hour.CompareTo(smsTimestamp.Hour);
							if (num == 0)
							{
								num = this.minute.CompareTo(smsTimestamp.Minute);
								if (num == 0)
								{
									num = this.second.CompareTo(smsTimestamp.Second);
									if (num == 0)
									{
										num = this.timeZoneOffset.CompareTo(smsTimestamp.TimeZoneOffset);
										return num;
									}
									else
									{
										return num;
									}
								}
								else
								{
									return num;
								}
							}
							else
							{
								return num;
							}
						}
						else
						{
							return num;
						}
					}
					else
					{
						return num;
					}
				}
				else
				{
					return num;
				}
			}
		}

		/// <summary>
		/// Returns the timestamp as a <see cref="T:System.DateTime" /> object.
		/// </summary>
		/// <returns>The converted date object.</returns>
		/// <remarks>
		/// <para>The <see cref="T:System.DateTime" /> object does not hold the time zone offset,
		/// this information will be lost when using this method. Use the
		/// TimeZoneOffset-releated properties and methods for working with the
		/// offset. If you just need the string representation, consider using the <see cref="M:GsmComm.PduConverter.SmsTimestamp.ToString" />,
		/// <see cref="M:GsmComm.PduConverter.SmsTimestamp.ToString(System.Boolean)" /> or <see cref="M:GsmComm.PduConverter.SmsTimestamp.ToSortableString" /> methods instead.
		/// </para>
		/// <para>
		/// An <see cref="T:GsmComm.PduConverter.SmsTimestamp" /> saves its years only with two digits by design, <see cref="T:System.DateTime" />
		/// uses 4 digits. The following conversion is performed when converting from an
		/// <see cref="T:GsmComm.PduConverter.SmsTimestamp" /> to a <see cref="T:System.DateTime" />: If the year is equal or greater than 90,
		/// then 1900 is added, otherwise 2000.
		/// </para>
		/// <para>
		/// If there is an error during conversion, the constant <see cref="F:System.DateTime.MinValue" />
		/// is returned, no exception is thrown.
		/// </para>
		/// </remarks>
		public DateTime ToDateTime()
		{
			DateTime dateTime;
			int num;
			try
			{
				if (this.year >= 90)
				{
					num = 1900 + this.year;
				}
				else
				{
					num = 2000 + this.year;
				}
				dateTime = new DateTime(num, this.month, this.day, this.hour, this.minute, this.second);
			}
			catch (ArgumentOutOfRangeException argumentOutOfRangeException)
			{
				dateTime = DateTime.MinValue;
			}
			return dateTime;
		}

		/// <summary>
		/// Returns a formatted date using the sortable date/time pattern and the time zone.
		/// </summary>
		/// <returns>The formatted date string.</returns>
		/// <remarks>The format is independent of the currently active culture and
		/// is always "yyyy-MM-ddTHH:mm:sszzz". It is useful for persisting the timestamp value as text.
		/// If you just want to display the value, consider using the <see cref="M:GsmComm.PduConverter.SmsTimestamp.ToString" /> or
		/// <see cref="M:GsmComm.PduConverter.SmsTimestamp.ToString(System.Boolean)" /> methods instead.</remarks>
		public string ToSortableString()
		{
			DateTime dateTime = this.ToDateTime();
			return string.Concat(dateTime.ToString("s"), this.TimeZoneOffsetSortableString);
		}

		/// <summary>
		/// Returns the formatted date using DateTime of the current culture + the time zone offset.
		/// </summary>
		/// <returns>The formatted date string.</returns>
		/// <remarks>
		/// The returned string is useful for display but not for persistence because is uses
		/// the currently active culture to format the string, which can cause problems when trying to parse
		/// the persisted string. If you want to persist the timestamp, consider using
		/// <see cref="M:GsmComm.PduConverter.SmsTimestamp.ToSortableString" /> instead.
		/// </remarks>
		public override string ToString()
		{
			return this.ToString(true);
		}

		/// <summary>
		/// Returns a formatted date string using DateTime of the current culture.
		/// </summary>
		/// <param name="includeTimeZoneOffset">Specify true to include the
		/// time zone offset in the string, false if it should not be included.</param>
		/// <returns>The formatted date string.</returns>
		/// <remarks>
		/// The returned string is useful for display but not for persistence because is uses
		/// the currently active culture to format the string, which can cause problems when trying to parse
		/// the persisted string. If you want to persist the timestamp, consider using
		/// <see cref="M:GsmComm.PduConverter.SmsTimestamp.ToSortableString" /> instead.
		/// </remarks>
		public string ToString(bool includeTimeZoneOffset)
		{
			string str;
			DateTime dateTime = this.ToDateTime();
			string str1 = dateTime.ToString();
			if (includeTimeZoneOffset)
			{
				str = string.Concat(" ", this.TimeZoneOffsetString);
			}
			else
			{
				str = "";
			}
			return string.Concat(str1, str);
		}
	}
}