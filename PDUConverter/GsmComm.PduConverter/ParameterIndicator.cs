using System;

/// <summary>
/// TP-PI / TP-Parameter-Indicator. Represents particular optional parameter
/// presence in the fields which follow.
/// </summary>
namespace GsmComm.PduConverter
{
	public class ParameterIndicator
	{
		private const byte bit0 = 1;

		private const byte bit1 = 2;

		private const byte bit2 = 4;

		private const byte bit3 = 8;

		private const byte bit4 = 16;

		private const byte bit5 = 32;

		private const byte bit6 = 64;

		private const byte bit7 = 128;

		private bool tp_pid;

		private bool tp_dcs;

		private bool tp_udl;

		private bool reserved_bit3;

		private bool reserved_bit4;

		private bool reserved_bit5;

		private bool reserved_bit6;

		private bool extension;

		/// <summary>
		/// When set to true, will indicate that another TP-PI octet follows immediately afterwards.
		/// </summary>
		public bool Extension
		{
			get
			{
				return this.extension;
			}
			set
			{
				this.extension = value;
			}
		}

		/// <summary>
		/// Reserved. If set to true, the receiving entity should ignore
		/// this setting.
		/// </summary>
		public bool Reserved_Bit3
		{
			get
			{
				return this.reserved_bit3;
			}
			set
			{
				this.reserved_bit3 = value;
			}
		}

		/// <summary>
		/// Reserved. If set to true, the receiving entity should ignore
		/// this setting.
		/// </summary>
		public bool Reserved_Bit4
		{
			get
			{
				return this.reserved_bit4;
			}
			set
			{
				this.reserved_bit4 = value;
			}
		}

		/// <summary>
		/// Reserved. If set to true, the receiving entity should ignore
		/// this setting.
		/// </summary>
		public bool Reserved_Bit5
		{
			get
			{
				return this.reserved_bit5;
			}
			set
			{
				this.reserved_bit5 = value;
			}
		}

		/// <summary>
		/// Reserved. If set to true, the receiving entity should ignore
		/// this setting.
		/// </summary>
		public bool Reserved_Bit6
		{
			get
			{
				return this.reserved_bit6;
			}
			set
			{
				this.reserved_bit6 = value;
			}
		}

		/// <summary>
		/// When true, a TP-DCS field is present.
		/// </summary>
		public bool TP_DCS
		{
			get
			{
				return this.tp_dcs;
			}
			set
			{
				this.tp_dcs = value;
			}
		}

		/// <summary>
		/// When true, a TP-PID field is present.
		/// </summary>
		public bool TP_PID
		{
			get
			{
				return this.tp_pid;
			}
			set
			{
				this.tp_pid = value;
			}
		}

		/// <summary>
		/// When false, neither TP-UDL nor TP-UD field can be present.
		/// </summary>
		public bool TP_UDL
		{
			get
			{
				return this.tp_udl;
			}
			set
			{
				this.tp_udl = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.ParameterIndicator" />.
		/// </summary>
		/// <param name="value">The value to initialize the object with.</param>
		public ParameterIndicator(byte value)
		{
			this.tp_pid = (value & 1) > 0;
			this.tp_dcs = (value & 2) > 0;
			this.tp_udl = (value & 4) > 0;
			this.reserved_bit3 = (value & 8) > 0;
			this.reserved_bit4 = (value & 16) > 0;
			this.reserved_bit5 = (value & 32) > 0;
			this.reserved_bit6 = (value & 64) > 0;
			this.extension = (value & 128) > 0;
		}

		public static implicit operator Byte(ParameterIndicator pi)
		{
			return pi.ToByte();
		}

		public static implicit operator ParameterIndicator(byte b)
		{
			return new ParameterIndicator(b);
		}

		/// <summary>
		/// Returns the byte equivalent of this instance.
		/// </summary>
		/// <returns>The byte value.</returns>
		public byte ToByte()
		{
			byte num = 0;
			if (this.tp_pid)
			{
				num = (byte)(num | 1);
			}
			if (this.tp_dcs)
			{
				num = (byte)(num | 2);
			}
			if (this.tp_udl)
			{
				num = (byte)(num | 4);
			}
			if (this.reserved_bit3)
			{
				num = (byte)(num | 8);
			}
			if (this.reserved_bit4)
			{
				num = (byte)(num | 16);
			}
			if (this.reserved_bit5)
			{
				num = (byte)(num | 32);
			}
			if (this.reserved_bit6)
			{
				num = (byte)(num | 64);
			}
			if (this.extension)
			{
				num = (byte)(num | 128);
			}
			return num;
		}
	}
}