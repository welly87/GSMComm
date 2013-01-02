using System;

/// <summary>
/// Provides data for the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.LoglineAdded" /> event.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class LoglineAddedEventArgs
	{
		private LogLevel level;

		private string text;

		/// <summary>
		/// Gets the log level.
		/// </summary>
		public LogLevel Level
		{
			get
			{
				return this.level;
			}
		}

		/// <summary>
		/// Gets the log text.
		/// </summary>
		public string Text
		{
			get
			{
				return this.text;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="level">The log level.</param>
		/// <param name="text">The log text.</param>
		public LoglineAddedEventArgs(LogLevel level, string text)
		{
			if (Enum.IsDefined(typeof(LogLevel), level))
			{
				if (text != null)
				{
					this.level = level;
					this.text = text;
					return;
				}
				else
				{
					throw new ArgumentNullException("text");
				}
			}
			else
			{
				throw new ArgumentException(string.Concat("Invalid log level \"", level, "\"."));
			}
		}
	}
}