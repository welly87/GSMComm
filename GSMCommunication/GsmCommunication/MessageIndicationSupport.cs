using System;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Contains information about the supported new message indications of the phone.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class MessageIndicationSupport
	{
		private string mode;

		private string deliver;

		private string cellBroadcast;

		private string statusReport;

		private string buffer;

		/// <summary>
		/// Gets a string representation of the supported buffer handling settings.
		/// </summary>
		public string BufferHandling
		{
			get
			{
				return this.buffer;
			}
		}

		/// <summary>
		/// Gets a string representation of the supported cell broadcast indication styles.
		/// </summary>
		public string CellBroadcastStyles
		{
			get
			{
				return this.cellBroadcast;
			}
		}

		/// <summary>
		/// Gets a string representation of the supported deliver indication modes.
		/// </summary>
		public string DeliverStyles
		{
			get
			{
				return this.deliver;
			}
		}

		/// <summary>
		/// Gets a string representation of the supported indication modes.
		/// </summary>
		public string Modes
		{
			get
			{
				return this.mode;
			}
		}

		/// <summary>
		/// Gets a string representation of the supported status report indication styles.
		/// </summary>
		public string StatusReportStyles
		{
			get
			{
				return this.statusReport;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="mode">A string representation of the phone's supported indication modes</param>
		/// <param name="deliver">A string representation of the phones's supported standard SMS (SMS-DELIVER) styles</param>
		/// <param name="cellBroadcast">A string representation of the phones's supported cell broadcast styles</param>
		/// <param name="statusReport">The phones's supported status report (SMS-STATUS-REPORT) styles</param>
		/// <param name="buffer">The phones's supported buffer handling settings</param>
		public MessageIndicationSupport(string mode, string deliver, string cellBroadcast, string statusReport, string buffer)
		{
			this.mode = mode;
			this.deliver = deliver;
			this.cellBroadcast = cellBroadcast;
			this.statusReport = statusReport;
			this.buffer = buffer;
		}

		private ArrayList ParseArrayAsString(string s)
		{
			ArrayList arrayLists = new ArrayList();
			Regex regex = new Regex("(?:(\\d+),?)+");
			Match match = regex.Match(s);
			if (match.Success)
			{
				foreach (Capture capture in match.Groups[1].Captures)
				{
					arrayLists.Add(int.Parse(capture.Value));
				}
			}
			Regex regex1 = new Regex("(\\d+)-(\\d+)");
			Match match1 = regex1.Match(s);
			if (match1.Success)
			{
				int num = int.Parse(match1.Groups[1].Value);
				int num1 = int.Parse(match1.Groups[2].Value);
				for (int i = num; i <= num1; i++)
				{
					arrayLists.Add(i);
				}
			}
			return arrayLists;
		}

		/// <summary>
		/// Checks if a specific buffer handling setting is supported.
		/// </summary>
		/// <param name="setting">The setting to check</param>
		/// <returns>true if the setting is supported, false otherwise.</returns>
		public bool SupportsBufferSetting(int setting)
		{
			ArrayList arrayLists = this.ParseArrayAsString(this.buffer);
			return arrayLists.Contains(setting);
		}

		/// <summary>
		/// Checks if a specific buffer handling setting is supported.
		/// </summary>
		/// <param name="setting">The setting to check</param>
		/// <returns>true if the setting is supported, false otherwise.</returns>
		public bool SupportsBufferSetting(IndicationBufferSetting setting)
		{
			return this.SupportsBufferSetting((int)setting);
		}

		/// <summary>
		/// Checks if a specific cell broadcast indication style is supported.
		/// </summary>
		/// <param name="style">The style to check</param>
		/// <returns>true if the style is supported, false otherwise.</returns>
		public bool SupportsCellBroadcastStyle(int style)
		{
			ArrayList arrayLists = this.ParseArrayAsString(this.cellBroadcast);
			return arrayLists.Contains(style);
		}

		/// <summary>
		/// Checks if a specific cell broadcast indication style is supported.
		/// </summary>
		/// <param name="style">The style to check</param>
		/// <returns>true if the style is supported, false otherwise.</returns>
		public bool SupportsCellBroadcastStyle(CbmIndicationStyle style)
		{
			return this.SupportsCellBroadcastStyle(style);
		}

		/// <summary>
		/// Checks if a specific SMS-DELIVER indication style is supported.
		/// </summary>
		/// <param name="style">The style to check</param>
		/// <returns>true if the style is supported, false otherwise.</returns>
		public bool SupportsDeliverStyle(int style)
		{
            
			ArrayList arrayLists = this.ParseArrayAsString(this.deliver);
			return arrayLists.Contains(style);
		}

		/// <summary>
		/// Checks if a specific SMS-DELIVER indication style is supported.
		/// </summary>
		/// <param name="style">The style to check</param>
		/// <returns>true if the style is supported, false otherwise.</returns>
		public bool SupportsDeliverStyle(SmsDeliverIndicationStyle style)
		{
            
			return this.SupportsDeliverStyle((int)style);
		}

		/// <summary>
		/// Checks if a specific indication mode is supported.
		/// </summary>
		/// <param name="mode">The mode to check</param>
		/// <returns>true if the mode is supported, false otherwise.</returns>
		public bool SupportsMode(int mode)
		{
            ArrayList arrayLists = this.ParseArrayAsString(this.mode);
            return arrayLists.Contains(mode);
		}

		/// <summary>
		/// Checks if a specific indication mode is supported.
		/// </summary>
		/// <param name="mode">The mode to check</param>
		/// <returns>true if the mode is supported, false otherwise.</returns>
		public bool SupportsMode(MessageIndicationMode mode)
		{
			return this.SupportsMode((int)mode);
		}

		/// <summary>
		/// Checks if a specific status report (SMS-STATUS-REPORT) indication style is supported.
		/// </summary>
		/// <param name="style">The style to check</param>
		/// <returns>true if the style is supported, false otherwise.</returns>
		public bool SupportsStatusReportStyle(int style)
		{
			ArrayList arrayLists = this.ParseArrayAsString(this.statusReport);
			return arrayLists.Contains(style);
		}

		/// <summary>
		/// Checks if a specific status report (SMS-STATUS-REPORT) indication style is supported.
		/// </summary>
		/// <param name="style">The style to check</param>
		/// <returns>true if the style is supported, false otherwise.</returns>
		public bool SupportsStatusReportStyle(SmsStatusReportIndicationStyle style)
		{
			return this.SupportsStatusReportStyle((int)style);
		}
	}
}