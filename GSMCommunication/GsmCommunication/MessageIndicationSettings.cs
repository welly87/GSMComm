using System;

/// <summary>
/// Specifies the settings for new message notifications.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public struct MessageIndicationSettings
	{
		private int mode;

		private int mt;

		private int bm;

		private int ds;

		private int bfr;

		/// <summary>
		/// Specifies how the indication buffer should be handled when indications are activated, i.e.
		/// when <see cref="P:GsmComm.GsmCommunication.MessageIndicationSettings.Mode" /> is set to any value except <see cref="F:GsmComm.GsmCommunication.MessageIndicationMode.DoNotForward" />.
		/// </summary>
		/// <remarks>
		/// You can use one of the <see cref="T:GsmComm.GsmCommunication.IndicationBufferSetting" /> values to set this property.
		/// </remarks>
		public int BufferSetting
		{
			get
			{
				return this.bfr;
			}
			set
			{
				this.bfr = value;
			}
		}

		/// <summary>
		/// Specifies how new Cell Broadcast messages should be indicated.
		/// </summary>
		/// <remarks>
		/// You can use one of the <see cref="T:GsmComm.GsmCommunication.CbmIndicationStyle" /> values to set this property.
		/// </remarks>
		public int CellBroadcastStyle
		{
			get
			{
				return this.bm;
			}
			set
			{
				this.bm = value;
			}
		}

		/// <summary>
		/// Specifies how new SMS-DELIVER messages should be indicated.
		/// </summary>
		/// <remarks>
		/// You can use one of the <see cref="T:GsmComm.GsmCommunication.SmsDeliverIndicationStyle" /> values to set this property.
		/// </remarks>
		public int DeliverStyle
		{
			get
			{
				return this.mt;
			}
			set
			{
				this.mt = value;
			}
		}

		/// <summary>
		/// Specifies the general indication mode.
		/// </summary>
		/// <remarks>
		/// You can use one of the <see cref="T:GsmComm.GsmCommunication.MessageIndicationMode" /> values to set this property.
		/// </remarks>
		public int Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		/// <summary>
		/// Specifies how new SMS-STATUS-REPORT messages should be indicated.
		/// </summary>
		/// <remarks>
		/// You can use one of the <see cref="T:GsmComm.GsmCommunication.SmsStatusReportIndicationStyle" /> values to set this property.
		/// </remarks>
		public int StatusReportStyle
		{
			get
			{
				return this.ds;
			}
			set
			{
				this.ds = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the structure.
		/// </summary>
		/// <param name="mode">Specifies the general indication mode.</param>
		/// <param name="mt">Specifies how new SMS-DELIVER messages should be indicated.</param>
		/// <param name="bm">Specifies how new Cell Broadcast messages should be indicated.</param>
		/// <param name="ds">Specifies how new SMS-STATUS-REPORT messages should be indicated.</param>
		/// <param name="bfr">Specifies how the indication buffer should be handled when indications are activated, i.e.
		/// when <see cref="P:GsmComm.GsmCommunication.MessageIndicationSettings.Mode" /> is set to any value except <see cref="F:GsmComm.GsmCommunication.MessageIndicationMode.DoNotForward" />.</param>
		public MessageIndicationSettings(int mode, int mt, int bm, int ds, int bfr)
		{
			this.mode = mode;
			this.mt = mt;
			this.bm = bm;
			this.ds = ds;
			this.bfr = bfr;
		}

		/// <summary>
		/// Initializes a new instance of the structure.
		/// </summary>
		/// <param name="mode">Specifies the general indication mode.</param>
		/// <param name="mt">Specifies how new SMS-DELIVER messages should be indicated.</param>
		/// <param name="bm">Specifies how new Cell Broadcast messages should be indicated.</param>
		/// <param name="ds">Specifies how new SMS-STATUS-REPORT messages should be indicated.</param>
		/// <param name="bfr">Specifies how the indication buffer should be handled when indications are activated, i.e.
		/// when <see cref="P:GsmComm.GsmCommunication.MessageIndicationSettings.Mode" /> is set to any value except <see cref="F:GsmComm.GsmCommunication.MessageIndicationMode.DoNotForward" />.</param>
		public MessageIndicationSettings(MessageIndicationMode mode, SmsDeliverIndicationStyle mt, CbmIndicationStyle bm, SmsStatusReportIndicationStyle ds, IndicationBufferSetting bfr) : this(mode, mt, bm, ds, bfr)
		{
		}
	}
}