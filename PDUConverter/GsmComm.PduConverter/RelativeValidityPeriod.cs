using System;
using System.Text;

/// <summary>
/// The relative validity period gives the length of the validity period
/// counted from when the SMS-SUBMIT is received by the SC.
/// </summary>
namespace GsmComm.PduConverter
{
	public class RelativeValidityPeriod : ValidityPeriod
	{
		private byte @value;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.RelativeValidityPeriod" />.
		/// </summary>
		/// <param name="value">The byte value of the validity period. Use this
		/// if you have already calculated the validity yourself.</param>
		public RelativeValidityPeriod(byte value)
		{
			this.@value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.RelativeValidityPeriod" />.
		/// </summary>
		/// <param name="period">The validity period.</param>
		/// <remarks>
		/// There are some rules to note:
		/// <list type="bullet">
		/// <item><description>
		/// The smallest validity period is 5 minutes, 63 weeks the largest.
		/// </description></item>
		/// <item><description>
		/// Periods between 5 minutes and 12 hours can be specified in 5 minute steps.
		/// </description></item>
		/// <item><description>
		/// Periods between 12h30min and 24 hours can be specified in 30 minute steps.
		/// </description></item>
		/// <item><description>
		/// Periods between two days and 30 days can be specified in 1 day steps.
		/// </description></item>
		/// <item><description>
		/// Periods between 5 weeks and 63 weeks can be specified in 1 week (=7 days) steps.
		/// </description></item>
		/// </list>
		/// </remarks>
		/// <exception cref="T:System.ArgumentException">Validity timespan is invalid.</exception>
		public RelativeValidityPeriod(TimeSpan period)
		{
			byte num = 0;
			while (num <= 255)
			{
				TimeSpan timeSpan = RelativeValidityPeriod.ToTimeSpan(num);
				if (timeSpan.CompareTo(period) != 0)
				{
					num = (byte)(num + 1);
				}
				else
				{
					this.@value = num;
					return;
				}
			}
			throw new ArgumentException("Invalid validity timespan.");
		}

		private void AppendIfNonzero(StringBuilder str, int val, string suffix)
		{
			if (val > 0)
			{
				if (str.Length != 0)
				{
					str.Append(" ");
				}
				str.Append(val.ToString());
				str.Append(suffix);
			}
		}

		public static explicit operator RelativeValidityPeriod(TimeSpan ts)
		{
			return new RelativeValidityPeriod(ts);
		}

		public static implicit operator TimeSpan(RelativeValidityPeriod v)
		{
			return v.ToTimeSpan();
		}

		public static implicit operator Byte(RelativeValidityPeriod v)
		{
			return v.ToByte();
		}

		public static implicit operator RelativeValidityPeriod(byte b)
		{
			return new RelativeValidityPeriod(b);
		}

		/// <summary>
		/// Returns the byte equivalent of this instance.
		/// </summary>
		/// <returns>The byte value.</returns>
		public byte ToByte()
		{
			return this.@value;
		}

		/// <summary>
		/// Returns the string equivalent of this instance.
		/// </summary>
		public override string ToString()
		{
			TimeSpan timeSpan = this.ToTimeSpan();
			if (timeSpan.TotalHours != 24)
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.AppendIfNonzero(stringBuilder, timeSpan.Days, "d");
				this.AppendIfNonzero(stringBuilder, timeSpan.Hours, "h");
				this.AppendIfNonzero(stringBuilder, timeSpan.Minutes, "m");
				this.AppendIfNonzero(stringBuilder, timeSpan.Seconds, "s");
				this.AppendIfNonzero(stringBuilder, timeSpan.Milliseconds, "ms");
				return stringBuilder.ToString();
			}
			else
			{
				return "24h";
			}
		}

		/// <summary>
		/// Returns the TimeSpan equivalent of this instance.
		/// </summary>
		/// <returns>The TimeSpan value.</returns>
		public TimeSpan ToTimeSpan()
		{
			return RelativeValidityPeriod.ToTimeSpan(this.@value);
		}

		private static TimeSpan ToTimeSpan(byte value)
		{
			if (value < 0 || value > 143)
			{
				if (value < 144 || value > 167)
				{
					if (value < 168 || value > 196)
					{
						if (value < 197 || value > 255)
						{
							return TimeSpan.Zero;
						}
						else
						{
							return new TimeSpan((value - 192) * 7, 0, 0, 0);
						}
					}
					else
					{
						return new TimeSpan(value - 166, 0, 0, 0);
					}
				}
				else
				{
					return new TimeSpan(12, (value - 143) * 30, 0);
				}
			}
			else
			{
				return new TimeSpan(0, (value + 1) * 5, 0);
			}
		}
	}
}